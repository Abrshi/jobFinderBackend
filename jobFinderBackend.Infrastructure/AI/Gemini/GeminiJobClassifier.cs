using System.Text.Json;
using jobFinder.Application.Jobs.Classification;
using jobFinder.Application.Jobs.DTOs;

namespace jobFinderBackend.Infrastructure.AI.Gemini;

public sealed class GeminiJobClassifier : IJobClassifier
{
    private readonly GeminiClient _geminiClient;

    private static readonly HashSet<string> ValidCategories =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Software & Technology",
            "Data & Analytics",
            "Cybersecurity",
            "Design & Creative",
            "Marketing & Communications",
            "Sales & Business Development",
            "Customer Service",
            "Finance & Accounting",
            "Human Resources",
            "Administration",
            "Project & Operations",
            "Education",
            "Healthcare",
            "Legal",
            "Engineering",
            "Architecture",
            "Construction & Trades",
            "Science & Research",
            "Logistics & Transportation",
            "Manufacturing",
            "Hospitality & Tourism",
            "Agriculture",
            "Media & Entertainment",
            "Government & Public Sector",
            "Nonprofit & Social Services",
            "Other"
        };

    public GeminiJobClassifier(GeminiClient geminiClient)
    {
        _geminiClient = geminiClient;
    }

    public async Task<JobClassificationResult> ClassifyAsync(
        ExternalJobDto job,
        CancellationToken cancellationToken = default)
    {
        var prompt = $$"""
        Analyze this job posting and classify it accurately.

        Your task is to understand the actual nature of the job rather than simply matching keywords.

        Consider:
        - Job title
        - Job description
        - Responsibilities
        - Required skills
        - Qualifications
        - Employment information
        - Company information
        - Location

        Return ONLY valid JSON.
        Do not include markdown.
        Do not include explanations.
        Do not include comments.

        ============================================================
        JOB INFORMATION
        ============================================================

        JOB TITLE:
        {{(string.IsNullOrWhiteSpace(job.Title)
            ? "MISSING / UNKNOWN"
            : job.Title)}}

        JOB DESCRIPTION:
        {{(string.IsNullOrWhiteSpace(job.Description)
            ? "MISSING"
            : job.Description)}}

        SOURCE EMPLOYMENT TYPE:
        {{job.EmploymentType ?? "unknown"}}

        SOURCE EXPERIENCE LEVEL:
        {{job.ExperienceLevel ?? "unknown"}}

        SOURCE REMOTE TYPE:
        {{job.RemoteType ?? "unknown"}}

        COMPANY:
        {{job.CompanyName ?? "unknown"}}

        LOCATION:
        {{job.City ?? "unknown"}}, {{job.Country ?? "unknown"}}

        SOURCE SKILLS:
        {{(job.Skills != null && job.Skills.Count > 0
            ? string.Join(", ", job.Skills)
            : "none")}}

        ============================================================
        CATEGORY CLASSIFICATION
        ============================================================

        Determine the single most appropriate professional category
        for this job based on the actual nature and purpose of the work.

        Do not classify the job only from keywords or individual
        technologies.

        Understand the job title, responsibilities, required skills,
        qualifications, and overall purpose of the position before
        selecting a category.

        Choose EXACTLY ONE category from this list:

        Software & Technology
        Data & Analytics
        Cybersecurity
        Design & Creative
        Marketing & Communications
        Sales & Business Development
        Customer Service
        Finance & Accounting
        Human Resources
        Administration
        Project & Operations
        Education
        Healthcare
        Legal
        Engineering
        Architecture
        Construction & Trades
        Science & Research
        Logistics & Transportation
        Manufacturing
        Hospitality & Tourism
        Agriculture
        Media & Entertainment
        Government & Public Sector
        Nonprofit & Social Services
        Other

        ============================================================
        CATEGORY DECISION RULES
        ============================================================

        Use your own reasoning to determine the category.

        The category must represent the PRIMARY professional domain
        of the job.

        Do not classify a job based only on one technology, tool,
        software package, or skill mentioned in the posting.

        Determine what the person is actually being hired to do.

        Examples of reasoning:

        A software developer using React:
        => Software & Technology

        A backend developer using .NET:
        => Software & Technology

        A mobile developer using Flutter:
        => Software & Technology

        A data analyst using Excel:
        => Data & Analytics

        A machine learning engineer using Python:
        => Data & Analytics

        A cybersecurity analyst using Python:
        => Cybersecurity

        A nurse using medical software:
        => Healthcare

        An accountant using Excel:
        => Finance & Accounting

        A teacher using online teaching software:
        => Education

        A graphic designer using Photoshop:
        => Design & Creative

        A video editor using Premiere Pro:
        => Design & Creative or Media & Entertainment,
        depending on the primary nature of the job.

        A marketing specialist using Google Analytics:
        => Marketing & Communications

        A civil engineer using AutoCAD:
        => Engineering

        An architect using Revit:
        => Architecture

        A warehouse worker using inventory software:
        => Logistics & Transportation

        An electrician:
        => Construction & Trades

        A factory production worker:
        => Manufacturing

        ============================================================
        IMPORTANT CATEGORY RULE
        ============================================================

        When multiple professional areas appear in a job,
        determine which area represents the PRIMARY responsibility.

        For example, a company may be hiring a developer for a
        healthcare company.

        The job should be:

        Software & Technology

        NOT:

        Healthcare

        because the person's profession is software development.

        Similarly, if a healthcare organization hires a nurse who
        uses software, the job should be:

        Healthcare

        NOT:

        Software & Technology

        Classify the JOB, not the industry of the company.

        Do not create new categories.

        Do not return multiple categories.

        Do not return subcategories.

        Do not return technology names as categories.

        Do not return job titles as categories.

        Use "Other" only when the job genuinely does not fit any
        available category.

        ============================================================
        TITLE RULES
        ============================================================

        If the source title is meaningful:

        - Preserve its actual meaning.
        - Clean obvious formatting problems.
        - Do not unnecessarily change it.
        - Use a professional and concise title.

        If the source title is:

        - missing
        - "N/A"
        - "Job"
        - "Job Posting"
        - meaningless
        - badly translated
        - obviously corrupted

        infer a professional English title from the job description
        and responsibilities.

        Examples:

        "Need React developer urgently"
        => "React Developer"

        "Looking for someone to edit TikTok and YouTube videos"
        => "Video Editor"

        ============================================================
        OTHER FIELD RULES
        ============================================================

        companyName:

        Use the provided company name when available.

        If it is missing, infer the company name from the job
        description when there is enough evidence.

        Otherwise return null.

        country:

        Use the provided country when available.

        If missing, infer the country from the job text or location
        information when reasonably possible.

        Otherwise return null.

        city:

        Use the provided city when available.

        If missing, infer the city from the job text when reasonably
        possible.

        Otherwise return null.

        employmentType:

        Determine the employment type from the available information.

        Must be exactly one of:

        Full-time
        Part-time
        Contract
        Freelance
        Internship
        Temporary
        null

        experienceLevel:

        Determine the required experience level when possible.

        Must be exactly one of:

        Entry Level
        Junior
        Mid-Level
        Senior
        Executive
        null

        remoteType:

        Determine the work arrangement when possible.

        Must be exactly one of:

        On-site
        Remote
        Hybrid
        null

        educationLevel:

        Infer the education requirement only when it is explicitly
        stated or strongly supported by the job posting.

        Otherwise return null.

        gender:

        Must be exactly one of:

        ANY
        MALE
        FEMALE

        Use ANY unless the job explicitly requires a specific gender.

        minimumExperienceYears:

        Return the minimum explicitly required number of years.

        If the posting does not explicitly state a required number
        of years, return null.

        companyIndustry:

        Infer the company's primary industry when possible.

        This field describes the COMPANY'S INDUSTRY, not the job
        category.

        Otherwise return null.

        skills:

        Extract concise, standard professional and technical skills
        from the job posting.

        Examples:

        C#
        React
        Python
        SQL
        Project Management
        Graphic Design
        Accounting
        Customer Service
        AutoCAD

        Maximum 12 skills.

        Skills must be:

        - concise
        - meaningful
        - standardized
        - relevant to the job

        Do NOT include:

        - full sentences
        - job responsibilities
        - long descriptions
        - UI labels
        - instructions
        - duplicated skills
        - meaningless words

        ============================================================
        OUTPUT
        ============================================================

        Return exactly this JSON structure:

        {
          "title": "string",
          "category": "one category from the provided category list",
          "companyName": "string or null",
          "country": "string or null",
          "city": "string or null",
          "employmentType": "string or null",
          "experienceLevel": "string or null",
          "remoteType": "string or null",
          "educationLevel": "string or null",
          "gender": "ANY or MALE or FEMALE",
          "minimumExperienceYears": "number or null",
          "companyIndustry": "string or null",
          "skills": ["string"]
        }

        Return ONLY JSON.
        """;

        try
        {
            var json = await _geminiClient.GenerateJsonAsync(
                prompt,
                cancellationToken);

            json = CleanJson(json);

            var result =
                JsonSerializer.Deserialize<JobClassificationResult>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (result == null)
            {
                return CreateFallback(job);
            }

            NormalizeResult(result, job);

            return result;
        }
        catch
        {
            return CreateFallback(job);
        }
    }

    private static JobClassificationResult CreateFallback(
        ExternalJobDto job)
    {
        return new JobClassificationResult
        {
            Title = !string.IsNullOrWhiteSpace(job.Title)
                ? job.Title.Trim()
                : "Untitled Position",

            Category = "Other",

            CompanyName = job.CompanyName,

            Country = job.Country,

            City = job.City,

            EmploymentType = job.EmploymentType,

            ExperienceLevel = job.ExperienceLevel,

            RemoteType = job.RemoteType,

            Skills = (job.Skills ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(12)
                .ToList()
        };
    }

    private static void NormalizeResult(
        JobClassificationResult result,
        ExternalJobDto job)
    {
        if (string.IsNullOrWhiteSpace(result.Title))
        {
            result.Title =
                !string.IsNullOrWhiteSpace(job.Title)
                    ? job.Title.Trim()
                    : "Untitled Position";
        }

        /*
         * Validate the category returned by Gemini.
         *
         * Gemini must return one of our known categories.
         * If it returns something else, use Other.
         */
        if (string.IsNullOrWhiteSpace(result.Category))
        {
            result.Category = "Other";
        }
        else
        {
            var normalizedCategory = ValidCategories
                .FirstOrDefault(category =>
                    category.Equals(
                        result.Category.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            result.Category = normalizedCategory ?? "Other";
        }

        result.CompanyName =
            PickNonEmpty(job.CompanyName, result.CompanyName);

        result.Country =
            PickNonEmpty(job.Country, result.Country);

        result.City =
            PickNonEmpty(job.City, result.City);

        result.EmploymentType =
            PickNonEmpty(job.EmploymentType, result.EmploymentType);

        result.ExperienceLevel =
            PickNonEmpty(job.ExperienceLevel, result.ExperienceLevel);

        result.RemoteType =
            PickNonEmpty(job.RemoteType, result.RemoteType);

        if (string.IsNullOrWhiteSpace(result.Gender))
        {
            result.Gender = "ANY";
        }
        else
        {
            result.Gender = NormalizeGender(result.Gender);
        }

        var geminiSkills = result.Skills ?? [];
        var sourceSkills = job.Skills ?? [];

        result.Skills = sourceSkills
            .Concat(geminiSkills)
            .Select(s => s?.Trim())
            .Where(IsValidSkillString)
            .Select(s => s!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(12)
            .ToList();
    }

    private static string NormalizeGender(string gender)
    {
        return gender.Trim().ToUpperInvariant() switch
        {
            "MALE" => "MALE",
            "FEMALE" => "FEMALE",
            _ => "ANY"
        };
    }

    private static bool IsValidSkillString(string? skill)
    {
        if (string.IsNullOrWhiteSpace(skill))
        {
            return false;
        }

        var trimmed = skill.Trim();

        if (trimmed.Length < 2 || trimmed.Length > 50)
        {
            return false;
        }

        if (trimmed.Equals(
                "Select",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (trimmed.Equals("—", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (trimmed.Contains(
                "http://",
                StringComparison.OrdinalIgnoreCase) ||
            trimmed.Contains(
                "https://",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (trimmed.Contains("<") || trimmed.Contains(">"))
        {
            return false;
        }

        return true;
    }

    private static string? PickNonEmpty(
        string? primary,
        string? fallback)
    {
        if (!string.IsNullOrWhiteSpace(primary))
        {
            return primary.Trim();
        }

        return string.IsNullOrWhiteSpace(fallback)
            ? null
            : fallback.Trim();
    }

    private static string CleanJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        json = json.Trim();

        if (json.StartsWith("```"))
        {
            var firstNewLine = json.IndexOf('\n');

            if (firstNewLine >= 0)
            {
                json = json[(firstNewLine + 1)..];
            }

            if (json.EndsWith("```"))
            {
                json = json[..^3];
            }
        }

        return json.Trim();
    }
}