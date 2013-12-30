using System.Collections.Generic;

namespace Collab.Models
{
    public class PreviousModel
    {
        public List<PreviousCollab> Collabs { get; set; }
    }

    public class PreviousCollab
    {
        public bool HasThumbnail { get; set; }
        public string Id { get; set; }
    }
}