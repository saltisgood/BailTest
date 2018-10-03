using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class BailCond
    {
        public class TextBlock
        {
            public readonly bool Editable;
            public bool Updated
            {
                get; private set;
            }

            private String mValue;
            public String Value
            {
                get
                {
                    return mValue;
                }

                set
                {
                    Updated = true;
                    mValue = value;
                }
            }

            public TextBlock(String initialText, bool editable)
            {
                mValue = initialText;
                Editable = editable;
                Updated = false;
            }
        }

        public class Condition
        {
            public bool Enabled;
            public String ShortText { get; private set; }
            private List<TextBlock> mTextBlocks;

            internal Condition(String shortText, List<TextBlock> texts)
            {
                Enabled = false;
                ShortText = shortText;
                mTextBlocks = texts;
            }
        }

        public class ConditionFactory
        {
            private List<TextBlock> mTextBlocks;

            public ConditionFactory()
            {
                mTextBlocks = new List<TextBlock>();
            }

            public ConditionFactory AddTextBlock(TextBlock b)
            {
                mTextBlocks.Add(b);
                return this;
            }

            public Condition Create(String shortText)
            {
                return new Condition(shortText, mTextBlocks);
            }

            public void Reset()
            {
                mTextBlocks.Clear();
            }
        }

        private List<Condition> mConditions;
        public IReadOnlyList<Condition> Conditions
        {
            get
            {
                return mConditions;
            }
        }

        private BailCond()
        {
            mConditions = new List<Condition>();
        }

        public static BailCond CreateDefault()
        {
            var c = new BailCond();

            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(new TextBlock("He is to live at:", false))
                .AddTextBlock(new TextBlock("<address>", true))
                .Create("Live at"));
            return c;
        }
    }
}
