using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.IO;
using Wycademy.Core.Enums;
using Wycademy.Core.Models;

namespace KiranicoScraper
{
    static class SharpnessImageGenerator
    {
        private const string OUTPUT_DIR = "./SharpnessImages";

        private static readonly Rgba32[] SHARPNESS_COLOURS = new Rgba32[]
        {
            Rgba32.Red, Rgba32.Orange, Rgba32.Yellow, Rgba32.Green, Rgba32.Blue, Rgba32.White, Rgba32.Purple
        };

        static SharpnessImageGenerator()
        {
            if (Directory.Exists(OUTPUT_DIR))
            {
                foreach (var file in Directory.EnumerateFiles(OUTPUT_DIR))
                {
                    File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory(OUTPUT_DIR);
            }
        }

        public static void GenerateImage(Game game, int levelId, IList<WeaponSharpness> sharpnessLevels)
        {
            // TODO: logging

            // Various constants used in image generation.
            const int imageWidth = 150;
            const int sideBuffer = 10;
            const int topBuffer = 10;
            const int boxSeparator = 5;
            const int boxHeight = 15;

            // Calculate the height of the generated image.
            // (top & bottom buffers) + (height of a box * number of boxes to draw) + (height of each separator * number of separators to draw) 
            var imageHeight = topBuffer * 2 + boxHeight * sharpnessLevels.Count + boxSeparator * (sharpnessLevels.Count - 1);

            // Set the sum of sharpness values that make up a full sharpness bar, and the number of sharpness values per bar.
            float fullBar;
            int valueCount;
            switch (game)
            {
                case Game.Four:
                    fullBar = 90;
                    valueCount = 7;
                    break;
                case Game.Generations:
                    fullBar = 400;
                    valueCount = 6;
                    break;
                case Game.World: // World weapons are currently not implemented
                    return;
                default:
                    return;
            }

            // Create the ImageSharp bitmap to draw on and a file stream to write the output to.
            using (var image = new Image<Rgba32>(imageWidth, imageHeight))
            using (var file = File.Create(Path.Combine(OUTPUT_DIR, $"{levelId}.png")))
            {
                image.Mutate(ctx =>
                {
                    ctx.BackgroundColor(Rgba32.DarkGray);

                    for (int i = 0; i < sharpnessLevels.Count; i++)
                    {
                        // Calculate the y position of the top left corner of the box.
                        // (top buffer) + (height of a box * boxes drawn so far) + (height of a separator * separators drawn so far)
                        var yPosition = topBuffer + boxHeight * i + boxSeparator * i;
                        ctx.Draw(Rgba32.Black, 3, new Rectangle(sideBuffer, yPosition, imageWidth - sideBuffer * 2, boxHeight));

                        // Add 1 to compensate for brush thickness when drawing the rectangle.
                        float xPosition = sideBuffer + 1;
                        for (int j = 0; j < valueCount; j++)
                        {
                            var colour = SHARPNESS_COLOURS[j];

                            // Scale the sharpness value based on the width of the box, compansating for the brush thickness.
                            // (width of box) * (sharpness value) / (sum of sharpness values that give a full bar)
                            float scaledWidth = (imageWidth - sideBuffer * 2 - 2) * sharpnessLevels[i][j] / fullBar;

                            // Draw a filled rectangle, compensating for brush thickness, with antialiasing turned off to avoid blending between colours.
                            ctx.Fill(new GraphicsOptions(enableAntialiasing: false), colour, new RectangleF(xPosition, yPosition + 1, scaledWidth, boxHeight - 2));

                            // Add the scaled width to the x position for the next iteration.
                            xPosition += scaledWidth;
                        }
                    }
                });

                image.SaveAsPng(file);
            }
        }
    }
}
