namespace ECTL
{
    using System;

    public class Read
    {
        public int ID { get; set; }
        public string EPC { get; set; }
        public string Time { get; set; }
        public string PeakRssiInDbm { get; set; }
        public string AntennaNumber { get; set; }
        public string ReaderNumber { get; set; }
        public string IpAddress { get; set; }
        public string UniqueReadingID { get; set; }
        public string TimingPoint { get; set; }

        public override string ToString()
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}#{5}#{6}#{7}#{8}@", ID, EPC, Time, PeakRssiInDbm, AntennaNumber, ReaderNumber, IpAddress, UniqueReadingID, TimingPoint);
        }
    }
}

