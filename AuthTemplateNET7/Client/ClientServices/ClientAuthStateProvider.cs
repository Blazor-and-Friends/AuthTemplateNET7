using Microsoft.AspNetCore.Components.Authorization;
using AuthTemplateNET7.Shared.SharedServices;
using AuthTemplateNET7.Shared.Dtos;
using AuthTemplateNET7.Shared.Dtos.Membership;
using System.Net.Http.Json;
using System.Security.Claims;

namespace AuthTemplateNET7.Client.ClientServices;

//added
public class ClientAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;

    public ClientAuthStateProvider(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        AuthedMemberDto authedMemberDto = await httpClient.GetFromJsonAsync<AuthedMemberDto>("/api/authstate/get-auth-state");

        ClaimsPrincipal claimsPrincipal;

        if (authedMemberDto is not null && authedMemberDto.Email is not null)
        {
            claimsPrincipal = new SharedAuthServices().CreateClaimsPrincipal("clientAuth", email: authedMemberDto.Email, roles: authedMemberDto.Roles, username: authedMemberDto.DisplayName);
        }
        else
        {
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new AuthenticationState(claimsPrincipal);
    }
}
