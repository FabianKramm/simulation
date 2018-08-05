using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Util.Dialog;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;

namespace Simulation.Util.UI.Elements
{
    public class TextView : UIElement
    {
        public static SpriteFont TextViewFont;

        public string Text
        {
            get; private set;
        }

        public bool IsEditable = true;
        private int relativeTop = 0;
        private int previousScrollWheelValue = 0;
        private Vector2 stringSize;
        private float lineHeight;
        private string displayText;

        public TextView(Rect bounds, string text)
        {
            Bounds = bounds;

            lineHeight = TextViewFont.MeasureString("LineHeight").Y;
            previousScrollWheelValue = SimulationGame.MouseState.ScrollWheelValue;

            SetText(text);
            OnClick(handleOnClick);
        }

        private void handleOnClick()
        {
            if(IsEditable)
            {
                var dialog = new InputDialog("Edit Text", Text);

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetText(dialog.ResultText);
                }
            }
        }

        public void SetText(string text)
        {
            Text = text;
            stringSize = TextViewFont.MeasureString(text);
            Bounds = new Rect(Bounds.GetPosition(), new Point((int)stringSize.X, (int)stringSize.Y));
            calculateDisplayText();
        }

        private void calculateDisplayText()
        {
            var linesToSkip = (int)(-relativeTop / lineHeight);
            var newLines = new List<string>();
            var oldLines = Text.Split('\n');

            for (int i = linesToSkip; i < oldLines.Length; i++)
                newLines.Add(oldLines[i]);

            displayText = newLines.Count == 0 ? "" : String.Join("\n", newLines);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var newScrollWheelValue = SimulationGame.MouseState.ScrollWheelValue;

            if (newScrollWheelValue != previousScrollWheelValue)
            {
                relativeTop = Math.Min(0, relativeTop + (int)(0.25f * (newScrollWheelValue - previousScrollWheelValue)));
                calculateDisplayText();
            }

            previousScrollWheelValue = newScrollWheelValue;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(TextViewFont, displayText, Bounds.GetPositionVector(), Color.White);
        }
    }
}
