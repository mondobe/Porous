namespace Porous
{
    public class PType { }

    public class PSignatureType : PType
    {
        public List<PType> ins, outs;

        public PSignatureType(List<PType> ins, List<PType> outs)
        {
            this.ins = ins;
            this.outs = outs;
        }
    }
}