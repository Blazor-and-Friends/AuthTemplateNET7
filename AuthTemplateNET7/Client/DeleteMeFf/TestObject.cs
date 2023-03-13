using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AuthTemplateNET7.Client.FormComponents;
using AuthTemplateNET7.Client.FormSampleObjs;
using AuthTemplateNET7.Shared.CustomAttributes;

namespace AuthTemplateNET7.Client.DeleteMeFf;

public class TestObject
{
    #region selects

    [FormFactorySelectSource(nameof(MicrosoftPeople))]
    [Display(Prompt = "Hello Dolly...")]
    public Guid FavoriteAspNetCommunityStandupRegularId { get; set; }

    [FormFactoryKeyValue(nameof(MicrosoftPerson.Id), nameof(MicrosoftPerson.Name))]
    public List<MicrosoftPerson> MicrosoftPeople { get; set; } = MicrosoftPerson.GetPeople();

    [FormFactorySelectSource(nameof(People))]
    public int PersonId { get; set; }

    [FormFactoryKeyValueAttribute(nameof(PersonWithInt.Id), nameof(PersonWithInt.Name))]
    public List<PersonWithInt> People { get; set; } = PersonWithInt.GetPeople();

    [Required]
    public CheckboxPresentationMode? CheckboxPresentationMode { get; set; }

    #endregion //selects

    #region dates/times

    [Required]
    public DateOnly? DOB { get; set; }// = DateOnly.FromDateTime(new DateTime(1969, 8, 12));

    public TimeOnly BirthTimeOfDay { get; set; }

    #endregion //dates/times

    #region numbers

    public byte? ThisIsAByte { get; set; }
    public short ThisIsAShort { get; set; }
    public int? ThisIsAnInt { get; set; }
    public long? ThisIsALong { get; set; }
    public float? ThisIsAFloat { get; set; }
    public double? ThisIsADouble { get; set; }
    public decimal? ThisIsADecimal { get; set; }

    #endregion //numbers



    [Display(Description = "This is a checkbox", Name = "Check... box")]
    [DataType("switch")] //todo DOCS how to make a checkbox look like a switch
    [Required]
    public bool? Checkbox { get; set; }

    [DisplayName("What time do you start your day?")]
    [Required]
    public TimeOnly? OpeningTime { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string FullName { get; set; }

    [Required, MinLength(3)]
    [DataType(DataType.MultilineText)]
    [Display(Description = "Hello Dolly!", Name = "Tell us about yourself", Prompt = "Start at the beginning...")]
    public string DescriptionOfYourself { get; set; }

    [DataType("color")] //todo DOCS to make a color field case doesn't matter, could be "coLoR"
    public string FavoriteColor { get; set; }
}
