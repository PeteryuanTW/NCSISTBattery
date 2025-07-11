using System.ComponentModel.DataAnnotations.Schema;

namespace NCSISTBattery.EFModel
{
    public partial class Jig
    {

        private Stack<JigContent> jigContent = new();
        [NotMapped]
        public List<JigContent> JigContent => jigContent.ToList();
        public void ImportContents(List<JigContent> newjigContents)
        {
            foreach (var newjigContent in newjigContents.OrderBy(x=>x.PushToStartTime))
            {
                jigContent.Push(newjigContent);
            }
        }

        public void PushJigContent(JigContent newjigContent)
        {
            jigContent.Push(newjigContent);
        }
    }
}
