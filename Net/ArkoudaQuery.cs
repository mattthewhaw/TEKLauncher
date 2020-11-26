﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TEKLauncher.ARK;
using TEKLauncher.Controls;
using TEKLauncher.Data;
using TEKLauncher.Pages;
using TEKLauncher.Servers;
using static System.Array;
using static System.BitConverter;
using static System.DateTime;
using static System.IO.File;
using static System.Text.Encoding;
using static System.Threading.Tasks.Task;
using static System.Windows.Application;
using static TEKLauncher.App;
using static TEKLauncher.Servers.ClustersManager;

namespace TEKLauncher.Net
{
    internal class ArkoudaQuery
    {
        private bool Expired;
        private List<IPAddress> IPs;
        internal readonly Dictionary<MapCode, byte[]> Checksums = new Dictionary<MapCode, byte[]>(9);
        private void ReadHashes(MemoryStream Stream, int Count)
        {
            for (int Iterator = 0; Iterator < Count; Iterator++)
            {
                byte[] Buffer = new byte[20];
                Stream.Read(Buffer, 0, 20);
                Checksums.Add((MapCode)Iterator, Buffer);
            }
        }
        private void ReadIPs(MemoryStream Stream, int Count)
        {
            byte[] Buffer = new byte[4];
            for (; Count > 0; Count--)
            {
                Stream.Read(Buffer, 0, 4);
                IPs.Add(new IPAddress(Buffer));
            }    
        }
        private void ReadServers(MemoryStream Stream)
        {
            int ServersCount = Stream.ReadByte();
            Clusters[0].Servers = new Server[ServersCount];
            for (int Iterator = 0; Iterator < ServersCount; Iterator++)
            {
                int IPIndex = Stream.ReadByte();
                MapCode Map = (MapCode)Stream.ReadByte();
                byte[] Buffer = new byte[2];
                Stream.Read(Buffer, 0, 2);
                int Port = ToUInt16(Buffer, 0), PlayersCount = Stream.ReadByte(), CustomNameLength = Stream.ReadByte();
                if (PlayersCount == 255 || Expired)
                    PlayersCount = -1;
                string CustomName = null;
                if (CustomNameLength != 0)
                {
                    Stream.Read(Buffer = new byte[CustomNameLength], 0, CustomNameLength);
                    CustomName = UTF8.GetString(Buffer);
                }
                (Clusters[0].Servers[Iterator] = new Server(IPs[IPIndex], Map, Port, CustomName)).Refresh(PlayersCount);
            }
            Current.Dispatcher.Invoke(RefreshClusterPage);
        }
        private void RefreshClusterPage()
        {
            if (Instance.CurrentPage is ClusterPage Page && IndexOf(Clusters, Page.Cluster) == 0)
            {
                Page.ServersList.Children.Clear();
                foreach (Server Server in Page.Cluster.Servers)
                    Page.ServersList.Children.Add(new ServerItem(Server));
            }
        }
        internal bool Request()
        {
            byte[] Query = new Downloader().TryDownloadData(Links.ArkoudaQuery);
            if (Query is null)
                return false;
            try
            {
                using (MemoryStream Reader = new MemoryStream(Query))
                {
                    byte[] Buffer = new byte[8];
                    Reader.Read(Buffer, 0, 8);
                    Expired = UtcNow.Ticks > ToInt64(Buffer, 0);
                    int SectionIndex;
                    while ((SectionIndex = Reader.ReadByte()) != -1)
                    {
                        Reader.Read(Buffer, 0, 2);
                        int BufferSize = ToInt16(Buffer, 0);
                        byte[] DataBuffer = new byte[BufferSize];
                        Reader.Read(DataBuffer, 0, BufferSize);
                        switch (SectionIndex)
                        {
                            case 0:
                                int IPsCount = BufferSize / 4;
                                IPs = new List<IPAddress>(IPsCount);
                                using (MemoryStream Stream = new MemoryStream(DataBuffer))
                                    try { ReadIPs(Stream, IPsCount); }
                                    catch { }
                                break;
                            case 1:
                                using (MemoryStream Stream = new MemoryStream(DataBuffer))
                                    try { ReadServers(Stream); }
                                    catch { }
                                break;
                            case 2:
                                using (MemoryStream Stream = new MemoryStream(DataBuffer))
                                    try { ReadHashes(Stream, BufferSize / 20); }
                                    catch { }
                                break;
                        }
                    }
                }
                return true;
            }
            catch (Exception Failure)
            {
                WriteAllText($@"{AppDataFolder}\QueryFailure.txt", $"{Failure.Message}\n{Failure.StackTrace}");
                return false;
            }
        }
        internal Task<bool> RequestAsync() => Run(Request);
    }
}