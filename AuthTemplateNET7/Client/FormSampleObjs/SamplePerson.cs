using AuthTemplateNET7.Client.FormComponents;
using System.ComponentModel.DataAnnotations;

namespace AuthTemplateNET7.Client.FormSampleObjs;

public class SamplePerson : IValidatableObject
{
    #region timeonly

    [Required]
    public TimeOnly? CurrentTime { get; set; }

    public TimeOnly OpeningTime { get; set; }

    #endregion //timeonly

    #region bools

    [Required(ErrorMessage = "Please let us know if you are over or under the age of 18.")]
    public bool? Over18 { get; set; }

    public bool OwnACar { get; set; } = true;

    #region Pizza toppings

    public bool ExtraCheese { get; set; }

    public bool ItalianSausge { get; set; }

    public bool Mushrooms { get; set; }

    #endregion //Pizza toppings


    #endregion //bools

    #region Dates

    [Required(ErrorMessage = "Please enter your date of birth")]
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

    [DataType(DataType.DateTime)]
    public DateTime FirstTimeIHadPizza { get; set; } = DateTime.Today.AddDays(-1);

    public DateTime? PageLoadedAt { get; set; }

    public DateTimeOffset WhenIMetMyMate { get; set; } = DateTimeOffset.Now.AddYears(-30);

    #endregion //Dates

    #region Numbers

    public int WholeDollarsInMyPocket { get; set; }

    #endregion //Numbers

    #region Selects

    public Guid? FavoriteAspNetCommunityStandupRegularId { get; set; } = Guid.Empty;

    public List<MicrosoftPerson> MicrosoftPeople { get; set; } = MicrosoftPerson.GetPeople();

    [Required(ErrorMessage = "Please select your favorite programming language")]
    public int? FavoriteLanguageId { get; set; }

    public List<ProgrammingLanguage> ProgrammingLanguages { get; set; } = ProgrammingLanguage.Get();

    [Required]
    public CheckboxPresentationMode? CheckboxPresentationMode { get; set; }

    #endregion

    #region InputText


    public string FavoriteColor { get; set; }

    [StringLength(5, MinimumLength = 3)]
    public string Name { get; set; }

    #endregion //InputText

    [DataType(DataType.MultilineText)]
    public string TellUsAboutYourself { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> result = new();

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        if(DateOfBirth > today)
        {
            ValidationResult validationResult = new ValidationResult("Please make sure your date of birth is today or in the past. If it's today, wow! You're a quick learner!", new string[] { "DateOfBirth" });
            result.Add(validationResult);
        }
        if(FirstTimeIHadPizza > DateTime.Today)
        {
            ValidationResult validationResult = new ValidationResult("Can't have eaten pizza in the future!", new string[] { "FirstTimeIHadPizza" });
        }

        return result;
    }
}
