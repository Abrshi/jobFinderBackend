using System.Text.Json.Serialization;

namespace jobFinder.Infrastructure.JobSources.Afriwork;

public sealed class AfriworkGraphQLRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; } = null!;

    [JsonPropertyName("variables")]
    public AfriworkGraphQLVariables Variables { get; set; } = new();
}

public sealed class AfriworkGraphQLVariables
{
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("whereCondition")]
    public AfriworkWhereCondition WhereCondition { get; set; } = new();

    [JsonPropertyName("orderCondition")]
    public List<AfriworkOrderCondition> OrderCondition { get; set; } = new();
}

public sealed class AfriworkWhereCondition
{
    [JsonPropertyName("_and")]
    public List<AfriworkWhereItem> And { get; set; } = new();
}

public sealed class AfriworkWhereItem
{
    [JsonPropertyName("approval_status")]
    public AfriworkApprovalStatusFilter ApprovalStatus { get; set; } = new();
}

public sealed class AfriworkApprovalStatusFilter
{
    [JsonPropertyName("_in")]
    public List<string> In { get; set; } = new();
}

public sealed class AfriworkOrderCondition
{
    [JsonPropertyName("latest_activity_at")]
    public string LatestActivityAt { get; set; } = "desc";
}

public sealed class AfriworkGraphQLResponse
{
    [JsonPropertyName("data")]
    public AfriworkGraphQLData? Data { get; set; }

    [JsonPropertyName("errors")]
    public List<AfriworkGraphQLError>? Errors { get; set; }
}

public sealed class AfriworkGraphQLData
{
    [JsonPropertyName("jobs")]
    public List<AfriworkJob> Jobs { get; set; } = new();
}

public sealed class AfriworkGraphQLError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public sealed class AfriworkJob
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("published_at")]
    public DateTime? PublishedAt { get; set; }

    [JsonPropertyName("refreshed_at")]
    public DateTime? RefreshedAt { get; set; }

    [JsonPropertyName("approval_status")]
    public string? ApprovalStatus { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("job_type")]
    public string? JobType { get; set; }

    [JsonPropertyName("job_site")]
    public string? JobSite { get; set; }

    [JsonPropertyName("skill_requirements")]
    public List<AfriworkSkillRequirement> SkillRequirements { get; set; } = new();

    [JsonPropertyName("city")]
    public AfriworkCity? City { get; set; }

    [JsonPropertyName("sectors")]
    public List<AfriworkSectorWrapper> Sectors { get; set; } = new();

    [JsonPropertyName("deadline")]
    public DateTime? Deadline { get; set; }

    [JsonPropertyName("compensation_amount_cents")]
    public decimal? CompensationAmountCents { get; set; }

    [JsonPropertyName("compensation_type")]
    public string? CompensationType { get; set; }

    [JsonPropertyName("compensation_currency")]
    public string? CompensationCurrency { get; set; }

    [JsonPropertyName("experience_level")]
    public string? ExperienceLevel { get; set; }

    [JsonPropertyName("entity")]
    public AfriworkEntity? Entity { get; set; }
}

public sealed class AfriworkSkillRequirement
{
    [JsonPropertyName("skill")]
    public AfriworkSkill? Skill { get; set; }
}

public sealed class AfriworkSkill
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public sealed class AfriworkCity
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("country")]
    public AfriworkCountry? Country { get; set; }
}

public sealed class AfriworkCountry
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public sealed class AfriworkSectorWrapper
{
    [JsonPropertyName("sector")]
    public AfriworkSector? Sector { get; set; }
}

public sealed class AfriworkSector
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public sealed class AfriworkEntity
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}