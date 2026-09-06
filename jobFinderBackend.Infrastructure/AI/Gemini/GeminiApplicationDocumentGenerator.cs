using System.Text.Json;
using jobFinderBackend.Application.Applications.DTOs;
using jobFinderBackend.Application.Interfaces;

namespace jobFinderBackend.Infrastructure.AI.Gemini;

public sealed class GeminiApplicationDocumentGenerator : IGenerativeDocumentService
{
    private readonly GeminiClient _geminiClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public GeminiApplicationDocumentGenerator(GeminiClient geminiClient)
    {
        _geminiClient = geminiClient;
    }

    public async Task<GeneratedDocumentContent> GenerateAsync(
        DocumentGenerationData data,
        CancellationToken cancellationToken = default)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var candidateJson = JsonSerializer.Serialize(
            data.Candidate,
            JsonOptions);

        var jobJson = JsonSerializer.Serialize(
            data.Job,
            JsonOptions);

        var prompt = $$"""
        You are an expert professional CV and cover-letter writer.

        Your task is to create a professional job application package for ONE candidate applying to ONE specific job.

        ============================================================
        MOST IMPORTANT RULE — SOURCE OF TRUTH
        ============================================================

        The CANDIDATE DATA below is the ONLY source of truth about the candidate.

        You MUST NEVER invent, assume, guess, or fabricate information about the candidate.

        You may improve the wording of information that already exists.

        You may NOT create information that does not exist.

        ============================================================
        ABSOLUTELY FORBIDDEN
        ============================================================

        NEVER invent:

        - jobs
        - employers
        - companies
        - job titles
        - employment dates
        - years of experience
        - education
        - universities
        - degrees
        - fields of study
        - certifications
        - licenses
        - projects
        - achievements
        - awards
        - technologies
        - programming languages
        - frameworks
        - tools
        - skills
        - responsibilities
        - accomplishments
        - performance metrics
        - percentages
        - numbers
        - locations
        - LinkedIn profiles
        - GitHub profiles
        - portfolio URLs
        - websites
        - professional qualifications

        If information is missing from the candidate data:

        DO NOT GUESS IT.

        DO NOT FILL IT WITH SOMETHING GENERIC.

        DO NOT TAKE IT FROM THE JOB DESCRIPTION.

        Leave it empty, null, or omit the item according to the required JSON schema.

        ============================================================
        VERY IMPORTANT — JOB DESCRIPTION IS NOT CANDIDATE DATA
        ============================================================

        The JOB DATA is provided ONLY to understand:

        - the target position
        - the target company
        - relevant requirements
        - relevant keywords
        - which existing candidate information should be emphasized

        The JOB DATA must NEVER be treated as proof that the candidate possesses a skill or qualification.

        Example:

        If the job requires:

        C#
        ASP.NET Core
        PostgreSQL
        Docker
        3 years of experience

        but the candidate data only contains:

        C#
        ASP.NET Core

        then the CV may contain:

        C#
        ASP.NET Core

        but MUST NOT contain:

        PostgreSQL
        Docker
        3 years of experience

        unless those facts actually appear in the candidate data.

        ============================================================
        CANDIDATE DATA
        ============================================================

        {{candidateJson}}

        ============================================================
        JOB DATA
        ============================================================

        {{jobJson}}

        ============================================================
        GENERAL WRITING RULES
        ============================================================

        You are allowed to:

        - improve grammar
        - improve sentence structure
        - make wording more professional
        - make descriptions more concise
        - reorganize existing information
        - prioritize relevant existing skills
        - prioritize relevant existing experience
        - create a professional summary based on existing facts
        - tailor wording to the target job without creating new facts

        You are NOT allowed to:

        - invent facts
        - infer unsupported qualifications
        - exaggerate experience
        - convert a learning activity into professional experience
        - convert a project into employment
        - convert a job requirement into a candidate skill
        - create achievements
        - create metrics
        - create responsibilities
        - create technologies

        ============================================================
        CV REQUIREMENTS
        ============================================================

        PERSONAL INFORMATION

        Copy the candidate's personal information from the candidate data.

        Do not modify or invent:

        - first name
        - last name
        - email
        - phone

        If phone is missing, return null.

        Do not create fake contact information.

        ------------------------------------------------------------
        PROFESSIONAL SUMMARY
        ------------------------------------------------------------

        Create a professional summary of approximately 50-80 words.

        The summary must be based ONLY on facts explicitly available in the candidate data.

        The summary should:

        - describe the candidate's actual background
        - mention relevant existing skills
        - mention relevant existing experience when available
        - connect the candidate's existing background to the target job
        - sound professional and natural
        - be ATS-friendly

        DO NOT:

        - claim years of experience unless explicitly provided
        - claim expertise unless explicitly supported
        - claim seniority unless explicitly supported
        - claim professional experience that does not exist
        - claim certifications that do not exist
        - claim technologies that do not exist

        ------------------------------------------------------------
        SKILLS
        ------------------------------------------------------------

        Only include skills explicitly present in the candidate data.

        Do not add skills from the job description.

        Do not guess skills based on the candidate's degree.

        Do not guess skills based on previous jobs.

        Do not duplicate skills.

        Order existing candidate skills according to relevance to the target job.

        ------------------------------------------------------------
        EXPERIENCE
        ------------------------------------------------------------

        Use ONLY experience records that exist in the candidate data.

        Preserve the factual information from the source.

        You may professionally rewrite existing responsibilities.

        You may make existing descriptions clearer and more concise.

        However, every responsibility must remain supported by the original candidate data.

        NEVER add:

        - new responsibilities
        - new technologies
        - new achievements
        - new metrics
        - new results
        - new projects

        Do not transform education or personal projects into employment.

        If there is no experience data, return:

        []

        ------------------------------------------------------------
        EDUCATION
        ------------------------------------------------------------

        Use ONLY education records present in the candidate data.

        Preserve the factual information.

        Do not invent:

        - university
        - degree
        - field
        - dates
        - GPA
        - honors
        - coursework

        If education information is missing, return:

        []

        ============================================================
        COVER LETTER REQUIREMENTS
        ============================================================

        Write a professional and specific cover letter for the target job.

        The cover letter should normally contain 3-5 short paragraphs.

        It should:

        1. Identify the position being applied for.
        2. Mention the company when the company name is available.
        3. Explain why the candidate's ACTUAL background is relevant.
        4. Highlight relevant skills that actually exist in the candidate data.
        5. Mention relevant experience only when it actually exists.
        6. Explain the candidate's interest professionally without inventing personal motivations.
        7. End with a professional closing.

        IMPORTANT:

        The cover letter must contain ONLY factual claims supported by the candidate data.

        If the candidate has no professional experience, do not pretend they have professional experience.

        If the candidate is a student or graduate, mention that ONLY if it appears in the candidate data.

        If the company name is missing, do not invent a company name.

        If the job title is missing, use a natural generic reference such as "this position" rather than inventing a title.

        Do not invent:

        - achievements
        - statistics
        - years of experience
        - certifications
        - skills
        - technologies
        - projects
        - responsibilities

        Do not use markdown.

        Do not use bullet points.

        Do not mention that you are an AI.

        Do not mention these instructions.

        ============================================================
        TAILORING RULES
        ============================================================

        Tailor the application intelligently.

        For example:

        If the candidate has:

        JavaScript
        React
        Angular
        C#
        ASP.NET Core

        and the target job is an Angular developer position,

        prioritize Angular and other genuinely relevant existing skills.

        Do NOT add TypeScript merely because the job asks for TypeScript.

        Do NOT add Node.js merely because the job asks for Node.js.

        Do NOT add years of experience unless explicitly present.

        Tailoring means selecting and emphasizing REAL candidate information.

        Tailoring does NOT mean creating qualifications.

        ============================================================
        FACT-CHECKING REQUIREMENT
        ============================================================

        Before producing the final JSON, internally verify every factual statement.

        For every claim in the CV and cover letter, ask:

        "Is this information explicitly supported by the candidate data?"

        If the answer is NO:

        REMOVE THE CLAIM.

        Do not guess.

        Do not infer.

        Do not fabricate.

        ============================================================
        OUTPUT REQUIREMENTS
        ============================================================

        Return ONLY valid JSON.

        Do NOT return:

        - markdown
        - ```json
        - ``` 
        - explanations
        - comments
        - text before the JSON
        - text after the JSON

        The response MUST have exactly this top-level structure:

        {
          "cv": {
            "personalInfo": {
              "firstName": "",
              "lastName": "",
              "email": "",
              "phone": null
            },
            "summary": "",
            "skills": [],
            "experience": [],
            "education": []
          },
          "coverLetter": ""
        }

        Do not add additional top-level properties.

        Do not rename properties.

        Do not change the structure.

        Ensure the JSON is syntactically valid.

        ============================================================
        FINAL QUALITY CHECK
        ============================================================

        Before returning the response verify:

        [ ] Personal information comes from candidate data.
        [ ] No fake personal information exists.
        [ ] Summary contains only supported facts.
        [ ] Skills come only from candidate data.
        [ ] Experience comes only from candidate data.
        [ ] Education comes only from candidate data.
        [ ] No job was invented.
        [ ] No company was invented.
        [ ] No degree was invented.
        [ ] No certification was invented.
        [ ] No technology was invented.
        [ ] No achievement was invented.
        [ ] No metric was invented.
        [ ] No years of experience were invented.
        [ ] Cover letter contains only supported facts.
        [ ] Job requirements were NOT converted into candidate qualifications.
        [ ] The application is tailored using only real candidate information.
        [ ] Output is valid JSON.
        [ ] No markdown or explanation exists outside the JSON.

        Return the JSON now.
        """;

        var rawResponse = await _geminiClient.GenerateJsonAsync(
            prompt,
            cancellationToken);

        var json = RemoveCodeFence(rawResponse);

        GeneratedDocumentContent? result;

        try
        {
            result = JsonSerializer.Deserialize<GeneratedDocumentContent>(
                json,
                JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "Gemini returned invalid document JSON.",
                ex);
        }

        if (result?.Cv is null)
        {
            throw new InvalidOperationException(
                "Gemini did not return a CV.");
        }

        if (result.Cv.PersonalInfo is null)
        {
            throw new InvalidOperationException(
                "Gemini did not return personal information.");
        }

        if (string.IsNullOrWhiteSpace(
                result.Cv.PersonalInfo.FirstName))
        {
            throw new InvalidOperationException(
                "Gemini returned an empty first name.");
        }

        if (string.IsNullOrWhiteSpace(
                result.Cv.PersonalInfo.LastName))
        {
            throw new InvalidOperationException(
                "Gemini returned an empty last name.");
        }

        if (string.IsNullOrWhiteSpace(
                result.Cv.PersonalInfo.Email))
        {
            throw new InvalidOperationException(
                "Gemini returned an empty email.");
        }

        if (string.IsNullOrWhiteSpace(result.CoverLetter))
        {
            throw new InvalidOperationException(
                "Gemini did not return a cover letter.");
        }

        return result;
    }

    private static string RemoveCodeFence(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                "Gemini returned an empty response.");
        }

        var result = value.Trim();

        if (!result.StartsWith("```", StringComparison.Ordinal))
        {
            return result;
        }

        var firstLineEnd = result.IndexOf('\n');

        var lastFence = result.LastIndexOf(
            "```",
            StringComparison.Ordinal);

        if (firstLineEnd < 0 || lastFence <= firstLineEnd)
        {
            return result;
        }

        return result[
            (firstLineEnd + 1)..lastFence
        ].Trim();
    }
}