using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.SharedServices;

//added
/// <summary>
/// Checks that the password necessary characters. By default password must have upper/lower case letter, number, and can have any of these special characters: `~!@#$%^&*()_-[]{}|;:<>,./
/// </summary>
public class PasswordValidationService
{
    /// <summary>
    /// List of characters allowed in the password. Default is `~!@#$%^&*()_-[]{}|;:<>,./;
    /// </summary>
    public string AllowedSpecialCharacters { get; set; } = "`~!@#$%^&*()_-[]{}|;:<>,./";

    /// <summary>
    /// If true, password must have a lowercase letter
    /// </summary>
    public bool Lower { get; set; } = true;

    /// <summary>
    /// If true, password must have a number
    /// </summary>
    public bool Number { get; set; } = true;

    /// <summary>
    /// If true, password must have an uppercase letter
    /// </summary>
    public bool Upper { get; set; } = true;

    /// <summary>
    /// Checks whether the password meets the requirements
    /// </summary>
    /// <param name="password">The password to check</param>
    /// <returns>Whether the password meets your requirements</returns>
    public bool IsValid(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;

        bool hasLower = false;
        if (!Lower) hasLower = true;

        bool hasNumber = false;
        if (!Number) hasNumber = true;

        bool hasUpper = false;
        if (!Upper) hasUpper = true;

        bool hasAllowedSpecialCharacter = false;
        if (string.IsNullOrWhiteSpace(AllowedSpecialCharacters)) hasAllowedSpecialCharacter = true;

        for (int i = 0; i < password.Length; i++)
        {
            char c = password[i];

            if (!hasLower) hasLower = char.IsLower(c);
            if (!hasNumber) hasNumber = char.IsNumber(c);
            if (!hasUpper) hasUpper = char.IsUpper(c);
            if (!hasAllowedSpecialCharacter) hasAllowedSpecialCharacter = AllowedSpecialCharacters.Contains(c);
        }
        return hasLower && hasNumber && hasUpper && hasAllowedSpecialCharacter;
    }
}
