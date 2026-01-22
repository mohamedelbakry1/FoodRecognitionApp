using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Entities
{
    public class Image : BaseEntity<int>
    {
        #region Properties
        public string ImageUrl { get; set; } = null!;
        public DateTime UploadTime { get; set; }
        #endregion

        #region Image - UserAccount
        public int UserId { get; set; }
        public UserAccount UserAccount { get; set; } = null!;
        #endregion

        #region Image - RecognitionResult
        public ICollection<RecognitionResult> RecognitionResults { get; set; } = null!;
        #endregion
    }
}
