
using System.Text.Json;
using jobFinder.Application.Jobs.Classification;
using jobFinder.Application.Jobs.DTOs;

namespace jobFinderBackend.Infrastructure.AI.Gemini;

public sealed class GeminiJobClassifier : IJobClassifier
{
    private readonly GeminiClient _geminiClient;

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

        Return ONLY valid JSON.
        Do not include markdown.
        Do not include explanations.
        Do not include comments.

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

        Choose EXACTLY ONE category from this list:

        Web Development
        Backend Development
        Frontend Development
        Full Stack Development
        Mobile Development
        Software Engineering
        DevOps
        Cloud Engineering
        Cybersecurity
        Networking
        Database Administration
        Data Analysis
        Data Science
        Machine Learning
        Artificial Intelligence
        QA & Testing

        UI/UX Design
        Graphic Design
        Video Editing
        Photography
        Content Creation
        Animation

        Digital Marketing
        Social Media Marketing
        SEO
        Content Marketing
        Sales
        Customer Support
        Business Development

        Accounting
        Finance
        Human Resources
        Administration
        Project Management
        Operations

        Teaching
        Healthcare
        Legal
        Engineering
        Architecture
        Logistics & Transport
        Manufacturing & Trades

        Other

        ============================================================
        CATEGORY RULES
        ============================================================

        IMPORTANT:
        Choose the MOST SPECIFIC category that matches the actual job.

        Do NOT blindly classify all technology jobs as "Software Engineering".

        Examples:

        React Developer
        Next.js Developer
        Angular Developer
        Vue Developer
        Frontend Developer
        Front End Engineer
        Web Developer
        Website Developer
        PHP Web Developer
        WordPress Developer

        => Web Development or Frontend Development depending on the job.

        Backend Developer
        Node.js Developer
        .NET Developer
        C# Backend Developer
        Java Backend Developer
        Python Backend Developer
        API Developer

        => Backend Development

        Full Stack Developer
        Full Stack Engineer
        MERN Stack Developer
        MEAN Stack Developer
        Full Stack Web Developer

        => Full Stack Development

        React Native Developer
        Flutter Developer
        Android Developer
        iOS Developer
        Mobile App Developer

        => Mobile Development

        DevOps Engineer
        DevOps Specialist
        CI/CD Engineer
        Infrastructure Engineer

        => DevOps

        AWS Engineer
        Azure Cloud Engineer
        Google Cloud Engineer
        Cloud Architect
        Cloud Engineer

        => Cloud Engineering

        Cybersecurity Analyst
        Security Engineer
        SOC Analyst
        Information Security Specialist

        => Cybersecurity

        Network Administrator
        Network Engineer
        Network Technician

        => Networking

        Data Analyst
        Business Intelligence Analyst
        Reporting Analyst

        => Data Analysis

        Data Scientist
        Data Science Specialist

        => Data Science

        Machine Learning Engineer
        ML Engineer
        Deep Learning Engineer

        => Machine Learning

        AI Engineer
        Artificial Intelligence Engineer
        Generative AI Engineer
        NLP Engineer

        => Artificial Intelligence

        QA Engineer
        Quality Assurance Engineer
        Software Tester
        Test Automation Engineer

        => QA & Testing

        UI Designer
        UX Designer
        UX/UI Designer
        Product Designer

        => UI/UX Design

        Graphic Designer
        Brand Designer
        Visual Designer
        Logo Designer

        => Graphic Design

        Video Editor
        Video Editing Specialist
        Film Editor
        Short-form Video Editor

        => Video Editing

        Photographer
        Photography Specialist

        => Photography

        Social Media Manager
        Social Media Specialist
        Social Media Coordinator

        => Social Media Marketing

        SEO Specialist
        SEO Manager

        => SEO

        Digital Marketing Specialist
        Digital Marketing Manager
        Performance Marketing Specialist

        => Digital Marketing

        Accountant
        Accounting Officer
        Senior Accountant
        Junior Accountant

        => Accounting

        Financial Analyst
        Finance Officer
        Financial Manager

        => Finance

        HR Officer
        Human Resources Specialist
        Recruiter
        Talent Acquisition Specialist

        => Human Resources

        Teacher
        Lecturer
        Instructor
        Tutor
        Teaching Assistant

        => Teaching

        Customer Service Representative
        Customer Support Agent
        Call Center Agent
        Customer Care Specialist

        => Customer Support

        ============================================================
        TITLE RULES
        ============================================================

        If the source title is good:
        - Clean it up.
        - Preserve the actual job meaning.
        - Do not unnecessarily change it.

        If the source title is:
        - missing
        - "N/A"
        - "Job"
        - "Job Posting"
        - meaningless
        - badly translated

        Then infer a professional English title from the description.

        Examples:

        "Need React developer urgently"
        => "React Developer"

        "Looking for someone to edit TikTok and YouTube videos"
        => "Video Editor"

        ============================================================
        OTHER FIELD RULES
        ============================================================

        companyName:
        Infer from the description if the source company name is missing.

        country:
        Infer from the job text when possible.

        Example:
        Addis Ababa => Ethiopia

        city:
        Infer from the job text when possible.

        employmentType:
        Must be exactly one of:

        Full-time
        Part-time
        Contract
        Freelance
        Internship
        Temporary
        null

        experienceLevel:
        Must be exactly one of:

        Entry Level
        Junior
        Mid-Level
        Senior
        Executive
        null

        remoteType:
        Must be exactly one of:

        On-site
        Remote
        Hybrid
        null

        educationLevel:
        Infer when explicitly stated.
        Otherwise return null.

        gender:
        Must be exactly one of:

        ANY
        MALE
        FEMALE

        Use ANY unless the job explicitly requires a specific gender.

        minimumExperienceYears:
        Return the minimum explicitly required number of years.
        If not explicitly stated, return null.

        companyIndustry:
        Infer the company's industry when possible.
        Otherwise return null.

        skills:
        Return concise, standard industry technical and professional skills (e.g. "C#", "React", "Project Management", "SQL", "Graphic Design").
        Maximum 12 skills.
        DO NOT include long sentences, job responsibilities, UI labels, or generic non-skill text (e.g. avoid phrases like "Teaching Exceptional Children", "Select", or full sentence descriptions).
        Do not duplicate skills.

        ============================================================
        OUTPUT
        ============================================================

        Return exactly this JSON structure:

        {
          "title": "string",
          "category": "string",
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
                .Take(15)
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

        if (string.IsNullOrWhiteSpace(result.Category))
        {
            result.Category = "Other";
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

        var geminiSkills = result.Skills ?? [];
        var sourceSkills = job.Skills ?? [];

        result.Skills = sourceSkills
            .Concat(geminiSkills)
            .Select(s => s?.Trim())
            .Where(IsValidSkillString)
            .Select(s => s!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(15)
            .ToList();
    }

    private static bool IsValidSkillString(string? skill)
    {
        if (string.IsNullOrWhiteSpace(skill)) return false;
        var trimmed = skill.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 50) return false;
        if (trimmed.Equals("Select", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("—", StringComparison.OrdinalIgnoreCase)) return false;
        if (trimmed.Contains("http://", StringComparison.OrdinalIgnoreCase) || trimmed.Contains("https://", StringComparison.OrdinalIgnoreCase)) return false;
        if (trimmed.Contains("<") || trimmed.Contains(">")) return false;
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