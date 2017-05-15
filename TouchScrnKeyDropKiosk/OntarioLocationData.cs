﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyCabinetKiosk
{
    public enum BikeType
    {
        Bike, EBike
    }

    class OntarioLocationData : LocationData
    {
        internal override List<CustomerScreen> CustomerScreenOrderList { get; set; }
        internal override List<AdminScreenType> AdminScreenList { get; set; }
        internal override List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; } 

        public OntarioLocationData():base()
        {
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReturningQuestion, WhenToUse.Both, new string [] {"Are You Taking","Or Returning A Bike?"}));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.BikeSelection, WhenToUse.Both));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.UserIDEntry, WhenToUse.Both));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.BikeOutOfStockQuestion, WhenToUse.Both));
        }

        /// <summary>
        /// Header - generates the coma separated header string to match CSV_Entry
        /// </summary>
        public override string Header
        {
            get
            {
                return "Date, Time, Box number, Door opened, User ID, Bike Type, Returned";
            }
            internal set
            {
                Header = value;
            }
        }     

        public class OntarioTransactionData : LocationTransactionData
        {
            #region get/set fields for property accessors
            string accesscode { get; set; }
            int locklocation { get; set; }
            int boxnumber { get; set; }
            bool dooropened { get; set; }
            string cardnumber { get; set; }
            string cardname { get; set; }
            bool returning { get; set; }
            BikeType biketype { get; set; }
            string userid { get; set; }
            #endregion
            public override List<LocationTransactionData.TransactionDataObject> ObjectList { get; set; }
            public override string AccessCode { get { return accesscode; } set { accesscode = value; ObjectList[LocateIndexOfDataObjectByName("Access Code")].data = value; } }//Whatever piece of info(User ID in this case) is used to identify the key to be accessed 
            public override int LockLocation { get { return locklocation; } set { locklocation = value; ObjectList[LocateIndexOfDataObjectByName("Lock")].data = value; } }//Location of lock based upon relay board numbering 
            public override DateTime transActionTime { get; set; }  //Time of transaction - when class created
            public override int BoxNumber { get { return boxnumber; } set { boxnumber = value; ObjectList[LocateIndexOfDataObjectByName("Box")].data = value; } }    // The box number - as setup for this project
            public override bool DoorOpened { get { return dooropened; } set { dooropened = value; ObjectList[LocateIndexOfDataObjectByName("Door Opened")].data = value; } }  // True if a correct access data given - does not indicate that door was physically opened 
            public override string CardNumber { get { return cardnumber; } set { cardnumber = value; ObjectList[LocateIndexOfDataObjectByName("Card Number")].data = value; } }          // With card access - the last four digits of card number
            public override string CardName { get { return cardname; } set { cardname = value; ObjectList[LocateIndexOfDataObjectByName("Card Name")].data = value; } }           // Name of card holder - for ease of review transaction data
            public override bool ReturningKey { get { return returning; } set { returning = value; ObjectList[LocateIndexOfDataObjectByName("Return")].data = value; } }   //States if the transaction is taking or returning a key
            public override string UserID { get { return userid; } set { userid = value; ObjectList[LocateIndexOfDataObjectByName("UserID")].data = value; } }
            public BikeType BikeType { get { return (BikeType)ObjectList[LocateIndexOfDataObjectByName("BikeType")].data; } set { biketype = value; ObjectList[LocateIndexOfDataObjectByName("BikeType")].data = value; } }

            public OntarioTransactionData()
                : base()
            {
                AddAdditionalDataObjectsToList();
            }

            public override void ClearData()
            {
                base.ClearData();                
            }

            private void AddAdditionalDataObjectsToList()
            {
                ObjectList.Add(new TransactionDataObject("BikeType", biketype, true, true));
            }

            private int LocateIndexOfDataObjectByName(string name)
            {
                for (int i = 0; i < ObjectList.Count; i++)
                {
                    if (ObjectList[i].name == name)
                        return i;
                }
                return -1;
            }

            /// <summary>
            /// This method returns an exact data copy of the current object. However, it is 
            /// not the object itself so when this object is changed, the copy will not be.
            /// </summary>
            /// <returns></returns>
            public override LocationTransactionData HardCopy()
            {
                OntarioTransactionData copy = new OntarioTransactionData();

                copy.AccessCode = this.AccessCode;
                copy.CardName = this.CardName;
                copy.CardNumber = this.CardNumber;
                copy.BoxNumber = this.BoxNumber;
                copy.DoorOpened = this.DoorOpened;
                copy.LockLocation = this.LockLocation;
                copy.ReturningKey = this.ReturningKey;
                copy.UserID = this.UserID;
                copy.BikeType = this.BikeType;

                //The object list must be copied piece by piece or both objects will 
                //point to the same ObjectList.
                for (int i = 0; i < this.ObjectList.Count; i++)
                {
                    copy.ObjectList[i] = (TransactionDataObject)this.ObjectList[i].Clone();
                }
                return copy;
            }

            /// <summary>
            /// CSV_Entry - generate a comma separated data entry for a transaction
            /// </summary>
            /// <returns></returns>
            public override string CSV_Entry()
            {
                try
                {
                    return String.Format("{0},{1},{2:d},{3},{4},{5},{6}",
                                        Date, Time, BoxNumber, DoorOpened.ToString(), AccessCode, BikeType.ToString(), ReturningKey.ToString());
                }
                catch (Exception ex)
                {
                    Program.logEvent("error creating csv entry" + ex.Message);
                    return "transaction recording error";
                }
            }

            /// <summary>
            /// Returns an array of the data which is applicable to an Ontario transaction
            /// </summary>
            /// <returns></returns>
            public override string[] TransactionData()
            {
                return new string[] { Date, Time, BoxNumber.ToString(), DoorOpened.ToString(), AccessCode, BikeType.ToString(), ReturningKey.ToString() };
            }
        }
    }
}
