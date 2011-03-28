﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Services;
using Common.Services;
using Common.Slots;
using Common.Beans;
using Common.Util;
using System.Net.Sockets;

namespace Client
{

    public interface ISlotManager
    {
        bool StartReservation(ReservationRequest req);

        Dictionary<int, CalendarSlot> ReadCalendar();
    }

    public class SlotManager :  MarshalByRefObject, IBookingService, ISlotManager
    {

        private Dictionary<int, CalendarSlot> _calendar;
        private Dictionary<int, Reservation> _activeReservations;

        private string _userName;
        private int _port;

        private List<ServerMetadata> _servers;

        public SlotManager(string userName, int port, List<ServerMetadata> servers)
        {
            _calendar = new Dictionary<int, CalendarSlot>();
            _activeReservations = new Dictionary<int, Reservation>();
            _userName = userName;
            _port = port;
            _servers = servers;
        }

        public bool StartReservation(ReservationRequest req)
        {
            //Updates request with sequence number
            req.ReservationID = RetrieveSequenceNumber();

            //Create and populate local reservation
            Reservation res = CreateReservation(req, _userName, Helper.GetIPAddress(), _port);

            foreach (int slot in req.Slots)
            {
                ReservationSlot rs = new ReservationSlot(req.ReservationID, slot, ReservationSlotState.INITIATED);
                res.Slots[slot] = rs;
            }

            //Add reservation to map of active reservations
            _activeReservations[res.ReservationID] = res;

            //Retrieve user's metadata
            List<ClientMetadata> onlineUsers = new List<ClientMetadata>();
            ILookupService server = Helper.GetRandomServer(_servers);
            for(int i=0; i<req.Users.Count; i++){
                try {
                    string userID = req.Users[i];
                    ClientMetadata participantInfo = Helper.GetRandomServer(_servers).Lookup(userID);
                } catch (SocketException) {
                    //server has failed
                    //update server reference and decrease loop counter
                    server = Helper.GetRandomServer(_servers);
                    i--;
                }
            }

            if (onlineUsers.Count != req.Users.Count)
            {
                //Not all users are online, what to do!?
            }

            foreach(ClientMetadata clientMd in onlineUsers){


            }


            return false;
        }

        private int RetrieveSequenceNumber()
        {
            int seqNumber = -1;

            while (seqNumber == -1)
            {
                try {
                    seqNumber = Helper.GetRandomServer(_servers).NextSequenceNumber();
                } catch (SocketException) {
                    //server has failed
                    //will try to get another server in next iteration
                }
            }

            return seqNumber;
        }

        public Dictionary<int, CalendarSlot> ReadCalendar()
        {
            throw new NotImplementedException();
        }

        private Reservation CreateReservation(ReservationRequest req, string initiatorID, string initiatorIP, int initiatorPort)
        {
            Reservation thisRes = new Reservation();
            thisRes.ReservationID = req.ReservationID;
            thisRes.Description = req.Description;
            thisRes.Participants = req.Users;
            thisRes.InitiatorID = initiatorID;
            thisRes.InitiatorIP = initiatorIP;
            thisRes.InitiatorPort = initiatorPort;

            return thisRes;
        }

        List<ReservationSlot> IBookingService.InitReservation(ReservationRequest req, string initiatorID, string initiatorIP, int initiatorPort)
        {
            throw new NotImplementedException();
        }

        void IBookingService.BookSlot(int resID, int slotID)
        {
            throw new NotImplementedException();
        }

        bool IBookingService.BookReply(int resID, int slotID, string userID)
        {
            throw new NotImplementedException();
        }

        void IBookingService.PreCommit(int resId, int slotID)
        {
            throw new NotImplementedException();
        }

        bool IBookingService.PreCommitReply(int resId, int slotID, string userID)
        {
            throw new NotImplementedException();
        }

        void IBookingService.DoCommit(int resId, int slotID)
        {
            throw new NotImplementedException();
        }

        bool IBookingService.DoCommitReply(int resId, int slotID, string userID)
        {
            throw new NotImplementedException();
        }

        bool IBookingService.Abort(int resId, int slotID, string userID)
        {
            throw new NotImplementedException();
        }


    }
}
