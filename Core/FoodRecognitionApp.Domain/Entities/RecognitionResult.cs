using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Entities
{
    public class RecognitionResult
    {
        #region Properties
        public double Confidence_Score { get; set; }
        public decimal Estimated_Quantity { get; set; }
        #endregion

        #region RecognitionResult - Image
        public int ImageId { get; set; }
        public Image Image { get; set; } = null!;
        #endregion

        #region RecognitionResult - Food
        public int FoodId { get; set; }
        public Food Food { get; set; } = null!;
        #endregion
    }
}
