using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    class BailCond
    {
        public class GenderFormat : IFormattable
        {
            public enum EGender
            {
                Male, Female
            };

            public static GenderFormat Male { get { return new GenderFormat(EGender.Male); } }
            public static GenderFormat Female { get { return new GenderFormat(EGender.Female); } }

            public EGender Gender { get; set; }

            public GenderFormat() : this(EGender.Male)
            { }

            public GenderFormat(EGender iGender)
            {
                Gender = iGender;
            }

            public string ToString(string format, IFormatProvider formatProvider)
            {
                switch (format)
                {
                    case "g":
                        return Gender == EGender.Male ? "he" : "she";
                    case "G":
                        return Gender == EGender.Male ? "He" : "She";
                    case "p":
                        return Gender == EGender.Male ? "his" : "her";
                    case "P":
                        return Gender == EGender.Male ? "His" : "Her";
                    default:
                        throw new ArgumentException("Invalid format string");
                }
            }
        };

        public interface ICondElem
        {
            UIElement Create(GenderFormat iGender);
        };

        public class StaticText : ICondElem
        {
            public readonly String Value;

            public StaticText(String iValue)
            {
                Value = iValue;
            }

            public UIElement Create(GenderFormat iGender)
            {
                return new Label
                {
                    Margin = new Thickness(10),
                    Content = String.Format(Value, iGender)
                };
            }
        };

        public class InputText : ICondElem
        {
            public readonly int MinSize;

            private String mValue;
            public String Value
            {
                get
                {
                    return mValue;
                }
                private set
                {
                    mValue = value;
                }
            }

            public InputText(String iInitialValue = "", int iMinSize = 0)
            {
                mValue = iInitialValue;
                MinSize = 0;
            }

            public UIElement Create(GenderFormat iGender)
            {
                // Values are just placeholders
                int aMinSize = MinSize != 0 ? MinSize : 120;
                return new TextBox
                {
                    Height = 23,
                    TextWrapping = TextWrapping.Wrap,
                    Width = aMinSize
                };
            }
        };

        public class TextBlock
        {
            public readonly bool Editable;
            public bool Updated
            {
                get; private set;
            }

            public readonly int MinSize;

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

            public static TextBlock Static(String text) { return new TextBlock(text, false); }
            public static TextBlock Input(String iInitialText = "", int iMinSize = 0) { return new TextBlock(iInitialText, true, iMinSize); }
            public static TextBlock AddressInput
            {
                get
                {
                    return Input(iMinSize: 200);
                }
            }
            public static TextBlock NameInput
            {
                get
                {
                    return Input(iMinSize: 150);
                }
            }

            public TextBlock(String iInitialText, bool iEditable, int iMinSize = 0)
            {
                mValue = iInitialText;
                Editable = iEditable;
                MinSize = iMinSize;
                Updated = false;
            }
        }

        public class Condition
        {
            public bool Enabled;
            public String ShortText { get; private set; }
            private readonly List<TextBlock> mTextBlocks;
            public IReadOnlyList<TextBlock> TextBlocks
            {
                get
                {
                    return mTextBlocks;
                }
            }

            internal Condition(String shortText, List<TextBlock> texts)
            {
                Enabled = false;
                ShortText = shortText;
                mTextBlocks = texts;
            }
        }

        public class ConditionFactory
        {
            private readonly List<TextBlock> mTextBlocks;

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
                .AddTextBlock(TextBlock.Static("{0:G} is to be of good behaviour."))
                .Create("Good behaviour"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is to live at"))
                .AddTextBlock(TextBlock.AddressInput)
                .AddTextBlock(TextBlock.Static("."))
                .Create("Address"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is to report to"))
                .AddTextBlock(TextBlock.AddressInput)
                .AddTextBlock(TextBlock.Static("Police Station daily / each Mon Tues Wed Thurs Fri Sat Sun between the hours of"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("and"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("."))
                .Create("Report to"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is to appear at"))
                .AddTextBlock(TextBlock.AddressInput)
                .AddTextBlock(TextBlock.Static("Court on"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("and thereafter as required."))
                .Create("Appear at"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is not to drink alcohol or enter any premises in which alcohol is sold."))
                .Create("Alcohol"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is not to take any illegal or prescription drugs (other than a drug prescribed to the applicant by a doctor)."))
                .Create("Drugs"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("To comply with a curfew: the applicant is not to be absent from the address at which {0:g} is required to live between the " +
                "hours of"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("and"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("am except if {0:g} is in the company of"))
                .AddTextBlock(TextBlock.NameInput)
                .AddTextBlock(TextBlock.Static("."))
                .Create("Curfew"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is not to associate or communicate by any means (except through his lawyer) with"))
                .AddTextBlock(TextBlock.NameInput)
                .AddTextBlock(TextBlock.Static("."))
                .Create("Associate"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("Not to have any contact in any way (including via a third party) with"))
                .AddTextBlock(TextBlock.NameInput)
                .AddTextBlock(TextBlock.Static("."))
                .Create("Forbidden contact"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("To undertake a course of rehabilitation at "))
                .AddTextBlock(TextBlock.AddressInput)
                .AddTextBlock(TextBlock.Static(". {0:G} is to obey any reasonable direcion given by the person for the time being in charge." +
                "{0:G} is not to leave that institution until the rehabilitation program is completed except for the purpose of complying with " +
                "reporting conditions, for conferences with legal advisors or attending court."))
                .Create("Rehabilitation"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("The applicant is to travel from the correctional centre from which {0:g} is to be released " +
                "on bail in the company of"))
                .AddTextBlock(TextBlock.NameInput)
                .AddTextBlock(TextBlock.Static("who must be in attendance at the correctional centre before the applicant is released."))
                .Create("Travel"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is to surrender {0:p} passport to"))
                .AddTextBlock(TextBlock.NameInput)
                .Create("Passport Surrender"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is not to apply for any new passport or travel document."))
                .Create("Passport Application"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("{0:G} is not to go within "))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("of any point of departure from the Commonwealth of Australia."))
                .Create("Travel"));

            // security
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("The applicant is to enter into an agreement under which {0:g} agrees to forfeit $"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("if {0:g} fails to appear before court in accordance with the bail acknowledgment."))
                .Create("Bail forfeit"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("The applicant is to deposit $"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("and agree to forfeit it if he fails to appear before court in accordance with the bail acknowledgment."))
                .Create("Deposit bail money"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("The applicant is to deposit acceptable security as security for the payment of $"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("which he aggress to forfeit if he fails to appear before court in accordance with the bail acknowledgement."))
                .Create("Security"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("One (or more) acceptable person(s) is to enter into an agreement under which he/she agrees to forfeit $"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("if the applicant fails to appear before court in accordance with the bail acknowledgment. I find such acceptable " +
                "person to be"))
                .AddTextBlock(TextBlock.Input())
                .Create("Security person"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("One (or more) acceptable person(s) is to deposit $"))
                .AddTextBlock(TextBlock.Input())
                .AddTextBlock(TextBlock.Static("and agree to forfeit it if the applicant fails to appear before court in accordance with the bail acknowledgment. " +
                "I find such acceptable person to be"))
                .AddTextBlock(TextBlock.Input())
                .Create("Some other dude"));
            // character
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("One (or more) acceptable person(s) is to provide an acknowledgment that he/she is acquainted with the applicant and " +
                "that he/she regards the applicant as a responsible person who is likely to comply with the bail acknowledgment. I find such acceptable person to be"))
                .AddTextBlock(TextBlock.Input())
                .Create("Character acknowledgment"));
            // enforcement
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("Enforcement of curfew condition: To present himself at the front door at the direction of any police offer to confirm " +
                "compliance with the curfew condition. Such direction may only be given by a police officer who believes on reasonable grounds that it is necessary " +
                "to do so, having regard to the rights of other occupants of the premises to peace and privacy."))
                .Create("Curfew enforcement"));
            c.mConditions.Add(new ConditionFactory()
                .AddTextBlock(TextBlock.Static("Enforcement of drug/alcohol abstention condition: To undertake any testing at the direction of any police officer to " +
                "confirm compliance with the drug/alcohol abstention condition. Such direction may only be given by a police officer who believes on reasonable grounds " +
                "that the applicant may have consumed drugs/alcohol in breach of the bail acknowledgment. Such testing may only be non-invasive and carried out with " +
                "respect given to the applicant's privacy."))
                .Create("Drug enforcement"));
            
            return c;
        }
    }
}
