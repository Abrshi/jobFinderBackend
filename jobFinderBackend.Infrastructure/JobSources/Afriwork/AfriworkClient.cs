using System.Net.Http.Json;
using System.Text.Json;

namespace jobFinder.Infrastructure.JobSources.Afriwork;

public sealed class AfriworkClient
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AfriworkClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<AfriworkJob>> GetJobsAsync(
        int offset = 0,
        int limit = 1,
        CancellationToken cancellationToken = default)
    {
        var request = new AfriworkGraphQLRequest
        {
            Query = AfriworkGraphQL.GetAllJobs,

            Variables = new AfriworkGraphQLVariables
            {
                Offset = offset,
                Limit = limit,

                WhereCondition = new AfriworkWhereCondition
                {
                    And =
                    [
                        new AfriworkWhereItem
                        {
                            ApprovalStatus = new AfriworkApprovalStatusFilter
                            {
                                // In = ["a", "s"]
                                In = ["PUBLISHEDD", "REFRESHEDD"]
                            }
                        }
                    ]
                },

                OrderCondition =
                [
                    new AfriworkOrderCondition
                    {
                        LatestActivityAt = "desc"
                    }
                ]
            }
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "");

        httpRequest.Headers.TryAddWithoutValidation(
            "Accept",
            "application/graphql-response+json, application/graphql+json, application/json, text/event-stream, multipart/mixed");

        httpRequest.Headers.TryAddWithoutValidation(
            "Accept-Language",
            "en-US,en;q=0.9");

        httpRequest.Headers.TryAddWithoutValidation(
            "Origin",
            "https://afriworket.com");

        httpRequest.Headers.TryAddWithoutValidation(
            "Referer",
            "https://afriworket.com/");

        httpRequest.Headers.TryAddWithoutValidation(
            "X-Hasura-Role",
            "anonymous");

        httpRequest.Content = JsonContent.Create(
            request,
            options: JsonOptions);

        using var response = await _httpClient.SendAsync(
            httpRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<AfriworkGraphQLResponse>(
                JsonOptions,
                cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Afriwork returned an empty response.");
        }

        if (result.Errors is { Count: > 0 })
        {
            var errors = string.Join(
                "; ",
                result.Errors
                    .Where(e => !string.IsNullOrWhiteSpace(e.Message))
                    .Select(e => e.Message));

            throw new InvalidOperationException(
                $"Afriwork GraphQL error: {errors}");
        }

        return result.Data?.Jobs ?? [];
    }
}