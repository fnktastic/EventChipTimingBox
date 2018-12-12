namespace ECTL
{
    using System;

    public class Read
    {
        public int ID { get; set; } //Autoincrement ID
        public Guid UniqueID { get; set; } //Unique Reading ID
        public string TagID { get; set; } //Tag ID
        public string TimeStamp { get; set; } //Time stamp with milliseconds
        public string ReaderNo { get; set; } //Reader Number (1 or 2)
        public string AntennaID { get; set; } //Antennta of th reader which recieves the data
        public string IPAddress { get; set; } //IP Address of REader (maybe not requiered)

        public override string ToString()
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}#{5}#{6}@", ID, UniqueID, TagID, TimeStamp, ReaderNo, AntennaID, IPAddress);
        }
    }
}

