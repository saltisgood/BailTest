using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    class BailCond
    {
        public enum EGender
        {
            Male, Female
        };

        public class GenderFormat : IFormattable
        {
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
            UIElement Create(EGender iGender);
        };

        public class StaticText : ICondElem
        {
            public readonly String Value;

            public StaticText(String iValue)
            {
                Value = iValue;
            }

            public UIElement Create(EGender iGender)
            {
                return new Label
                {
                    Margin = new Thickness(10),
                    Content = String.Format(Value, new GenderFormat(iGender))
                };
            }
        };

        public class InputText : ICondElem
        {
            public static InputText Address
            {
                get
                {
                    return new InputText(iMinSize: 200);
                }
            }

            public static InputText Name
            {
                get
                {
                    return new InputText(iMinSize: 150);
                }
            }

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

            public UIElement Create(EGender iGender)
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

        public class Condition
        {
            public bool Enabled;
            public String ShortText { get; private set; }

            private readonly List<ICondElem> mCondElems;
            public IReadOnlyList<ICondElem> CondElems
            {
                get
                {
                    return mCondElems;
                }
            }

            internal Condition(String shortText, List<ICondElem> elems)
            {
                Enabled = false;
                ShortText = shortText;
                mCondElems = elems;
            }
        }

        public class ConditionFactory
        {
            private readonly List<ICondElem> mCondElems;

            public ConditionFactory()
            {
                mCondElems = new List<ICondElem>();
            }

            public ConditionFactory AddTextBlock(TextBlock b)
            {
                return this;
            }

            public ConditionFactory Add(ICondElem e)
            {
                mCondElems.Add(e);
                return this;
            }

            public Condition Create(String shortText)
            {
                return new Condition(shortText, mCondElems);
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
                .Add(new StaticText("{0:G} is to be of good behaviour."))
                .Create("Good behaviour"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is to live at"))
                .Add(InputText.Address)
                .Add(new StaticText("."))
                .Create("Address"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is to report to"))
                .Add(InputText.Address)
                .Add(new StaticText("Police Station daily / each Mon Tues Wed Thurs Fri Sat Sun between the hours of"))
                .Add(new InputText())
                .Add(new StaticText("and"))
                .Add(new InputText())
                .Add(new StaticText("."))
                .Create("Report to"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is to appear at"))
                .Add(InputText.Address)
                .Add(new StaticText("Court on"))
                .Add(new InputText())
                .Add(new StaticText("and thereafter as required."))
                .Create("Appear at"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is not to drink alcohol or enter any premises in which alcohol is sold."))
                .Create("Alcohol"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is not to take any illegal or prescription drugs (other than a drug prescribed to the applicant by a doctor)."))
                .Create("Drugs"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("To comply with a curfew: the applicant is not to be absent from the address at which {0:g} is required to live between the " +
                "hours of"))
                .Add(new InputText())
                .Add(new StaticText("and"))
                .Add(new InputText())
                .Add(new StaticText("am except if {0:g} is in the company of"))
                .Add(InputText.Name)
                .Add(new StaticText("."))
                .Create("Curfew"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is not to associate or communicate by any means (except through his lawyer) with"))
                .Add(InputText.Name)
                .Add(new StaticText("."))
                .Create("Associate"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("Not to have any contact in any way (including via a third party) with"))
                .Add(InputText.Name)
                .Add(new StaticText("."))
                .Create("Forbidden contact"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("To undertake a course of rehabilitation at "))
                .Add(InputText.Address)
                .Add(new StaticText(". {0:G} is to obey any reasonable direcion given by the person for the time being in charge." +
                "{0:G} is not to leave that institution until the rehabilitation program is completed except for the purpose of complying with " +
                "reporting conditions, for conferences with legal advisors or attending court."))
                .Create("Rehabilitation"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("The applicant is to travel from the correctional centre from which {0:g} is to be released " +
                "on bail in the company of"))
                .Add(InputText.Name)
                .Add(new StaticText("who must be in attendance at the correctional centre before the applicant is released."))
                .Create("Travel"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is to surrender {0:p} passport to"))
                .Add(InputText.Name)
                .Create("Passport Surrender"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is not to apply for any new passport or travel document."))
                .Create("Passport Application"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("{0:G} is not to go within "))
                .Add(new InputText())
                .Add(new StaticText("of any point of departure from the Commonwealth of Australia."))
                .Create("Travel"));

            // security
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("The applicant is to enter into an agreement under which {0:g} agrees to forfeit $"))
                .Add(new InputText())
                .Add(new StaticText("if {0:g} fails to appear before court in accordance with the bail acknowledgment."))
                .Create("Bail forfeit"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("The applicant is to deposit $"))
                .Add(new InputText())
                .Add(new StaticText("and agree to forfeit it if he fails to appear before court in accordance with the bail acknowledgment."))
                .Create("Deposit bail money"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("The applicant is to deposit acceptable security as security for the payment of $"))
                .Add(new InputText())
                .Add(new StaticText("which he aggress to forfeit if he fails to appear before court in accordance with the bail acknowledgement."))
                .Create("Security"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("One (or more) acceptable person(s) is to enter into an agreement under which he/she agrees to forfeit $"))
                .Add(new InputText())
                .Add(new StaticText("if the applicant fails to appear before court in accordance with the bail acknowledgment. I find such acceptable " +
                "person to be"))
                .Add(new InputText())
                .Create("Security person"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("One (or more) acceptable person(s) is to deposit $"))
                .Add(new InputText())
                .Add(new StaticText("and agree to forfeit it if the applicant fails to appear before court in accordance with the bail acknowledgment. " +
                "I find such acceptable person to be"))
                .Add(new InputText())
                .Create("Some other dude"));
            // character
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("One (or more) acceptable person(s) is to provide an acknowledgment that he/she is acquainted with the applicant and " +
                "that he/she regards the applicant as a responsible person who is likely to comply with the bail acknowledgment. I find such acceptable person to be"))
                .Add(new InputText())
                .Create("Character acknowledgment"));
            // enforcement
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("Enforcement of curfew condition: To present himself at the front door at the direction of any police offer to confirm " +
                "compliance with the curfew condition. Such direction may only be given by a police officer who believes on reasonable grounds that it is necessary " +
                "to do so, having regard to the rights of other occupants of the premises to peace and privacy."))
                .Create("Curfew enforcement"));
            c.mConditions.Add(new ConditionFactory()
                .Add(new StaticText("Enforcement of drug/alcohol abstention condition: To undertake any testing at the direction of any police officer to " +
                "confirm compliance with the drug/alcohol abstention condition. Such direction may only be given by a police officer who believes on reasonable grounds " +
                "that the applicant may have consumed drugs/alcohol in breach of the bail acknowledgment. Such testing may only be non-invasive and carried out with " +
                "respect given to the applicant's privacy."))
                .Create("Drug enforcement"));
            
            return c;
        }
    }
}
