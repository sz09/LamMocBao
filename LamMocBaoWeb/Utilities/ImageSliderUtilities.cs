using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LamMocBaoWeb.Utilities
{
    public enum SliderMode
    {
        ShowButtons,
        ShowPreview
    }

    public static class ImageSliderUtilities 
    {
        private static string IDENTIFY_DOM = "card_image__{0}__";
        private static string BACKGROUND_IMAGE_TEMPLATE = "<div id='{0}{1}' class='card__image mb-2 {3}' style='background-image: url({2});'></div>";
        private static string IMAGE_TEMPLATE = "<img height='{1}' width={2} src='{0}'/>";
        private static readonly int PREVIEW_BUTTON_WIDTH = 182; // Width per preview button + margin + 2px for

        public static string ToImageSlider(this IEnumerable<string> imageUrls, Guid identifyId, SliderMode sliderMode = SliderMode.ShowButtons)
        {
            if (!imageUrls.Any())
            {
                return string.Empty;
            }
            var identifyDom = string.Format(IDENTIFY_DOM, identifyId);
            StringBuilder stringBuilder = new StringBuilder();
            var shouldShowButtons = imageUrls.Count() > 1;
            if (sliderMode == SliderMode.ShowButtons && shouldShowButtons)
            {
                stringBuilder.AppendLine($"<a class='prev' onclick=\"ImageSlider.GoTo('{identifyDom}', -1)\">&#10094;</a>");
            }

            var html = string.Join(Environment.NewLine, imageUrls.Select((imageUrl, index) => {
                string display = "";
                if(index > 0)
                {
                    display = "d-none";
                }

                return string.Format(BACKGROUND_IMAGE_TEMPLATE, identifyDom, index, imageUrl, display);
            }));
            stringBuilder.AppendLine(html);

            switch (sliderMode)
            {
                case SliderMode.ShowButtons:
                    if (shouldShowButtons)
                        AppendShowButtons(stringBuilder, identifyDom);
                    break;
                case SliderMode.ShowPreview:
                    AppendPreviewImages(stringBuilder, identifyDom, imageUrls);
                    break;

            }

            return stringBuilder.ToString();
        }
        
        private static void AppendShowButtons(StringBuilder stringBuilder, string identifyDom)
        {
            stringBuilder.AppendLine($"<a class='next' onclick=\"ImageSlider.GoTo('{identifyDom}', 1)\">&#10095</a>");
        }

        private static void AppendPreviewImages(StringBuilder stringBuilder, string identifyDom, IEnumerable<string> imageUrls)
        {
            stringBuilder.AppendLine("<div class='images-slider'>");
            stringBuilder.AppendLine($"<div class='images-slider-content'>");
            for (int index = 0; index < imageUrls.Count(); index++)
            {
                string borderClass = index != 0 ? "" : "border-selected";
                var imageUrl = imageUrls.ElementAt(index);
                stringBuilder.AppendLine($"<a class='prev' onclick=\"ImageSlider.GoTo('{identifyDom}', -1)\">&#10094;</a>");
                stringBuilder.AppendLine($"<a class='next' onclick=\"ImageSlider.GoTo('{identifyDom}', 1)\">&#10095</a>");
                stringBuilder.AppendLine($"<div class='preview-button m-2 {borderClass}' id='preview-{identifyDom}{index}' style='background-image: url({imageUrl})' onclick=\"ImageSlider.ShowImage('{identifyDom}', {index})\"></div>");
            }
            stringBuilder.AppendLine("</div>");
            stringBuilder.AppendLine("</div>");
        }

        public static string ToImages(this IEnumerable<string> imageUrls, int height = 50, int width = 50)
        {
            if (!imageUrls.Any())
            {
                return string.Empty;
            }

            return string.Join(Environment.NewLine, imageUrls.Select(imageUrl => string.Format(IMAGE_TEMPLATE, imageUrl, height, width)));
        }

        public static string ToImage(this string imageUrl, int height = 50, int width = 50)
        {
            return string.Format(IMAGE_TEMPLATE, imageUrl, height, width);
        }
    }
}
