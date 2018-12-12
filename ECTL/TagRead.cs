namespace ECTL
{
    using Impinj.OctaneSdk;
    using System;

    internal sealed class TagRead
    {
        public readonly string EPC;
        public SpeedwayReader Reader;
        public Impinj.OctaneSdk.Tag Tag;
        public readonly DateTime UTC = DateTime.UtcNow;

        public TagRead(SpeedwayReader reader, string epc, Impinj.OctaneSdk.Tag tag)
        {
            this.Reader = reader;
            this.EPC = epc;
            this.Tag = tag;
        }
    }
}

