namespace jobFinderBackend.Infrastructure.Jobd;

public static class SkillCategoryResolver
{
    public static string ResolveCategory(string? skillName)
    {
        if (string.IsNullOrWhiteSpace(skillName))
        {
            return "Professional & General Skills";
        }

        var lower = skillName.Trim().ToLowerInvariant();

        // 1. Software Engineering & IT Infrastructure
        if (lower.Contains("software architecture") || lower.Contains("software installation") ||
            lower.Contains("software adapter") || lower.Contains("it asset management") ||
            lower.Contains("technology trend") || lower.Contains("software engineering") ||
            lower.Contains("software development"))
        {
            return "Software Engineering & IT";
        }

        // 2. AI & Data
        if (lower.Contains("machine learning") || lower.Contains("data analysis") || lower.Contains("data science") ||
            lower.Contains("tensorflow") || lower.Contains("pytorch") || lower.Contains("pandas") ||
            lower.Contains("numpy") || lower.Contains("artificial intelligence") || lower.Equals("ai") ||
            lower.Contains("ai &") || lower.Contains("ai ") || lower.Contains("data analytics") ||
            lower.Contains("business intelligence") || lower.Contains("big data") || lower.Contains("neural network"))
        {
            return "AI & Data";
        }

        // 3. Programming Languages
        if (lower.Contains("c#") || lower.Contains("c++") || lower.Contains("typescript") ||
            lower.Contains("javascript") || lower.Contains("python") || lower.Contains("java") ||
            lower.Contains("golang") || lower.Contains("rust") || lower.Contains("php") ||
            lower.Contains("kotlin") || lower.Contains("swift") || lower.Contains("ruby"))
        {
            return "Programming Languages";
        }

        // 4. Frontend & Web
        if (lower.Contains("react") || lower.Contains("angular") || lower.Contains("vue") ||
            lower.Contains("html") || lower.Contains("css") || lower.Contains("tailwind") ||
            lower.Contains("next.js") || lower.Contains("nuxt") || lower.Contains("bootstrap") ||
            lower.Contains("front-end") || lower.Contains("frontend"))
        {
            return "Frontend";
        }

        // 5. Backend
        if (lower.Contains("node") || lower.Contains(".net") || lower.Contains("asp.net") ||
            lower.Contains("express") || lower.Contains("spring") || lower.Contains("laravel") ||
            lower.Contains("django") || lower.Contains("fastapi") || lower.Contains("nest.js") ||
            lower.Contains("rest api") || lower.Contains("graphql") || lower.Contains("back-end") ||
            lower.Contains("backend"))
        {
            return "Backend";
        }

        // 6. Database
        if (lower.Contains("sql") || lower.Contains("postgres") || lower.Contains("mongo") ||
            lower.Contains("redis") || lower.Contains("database") || lower.Contains("oracle") ||
            lower.Contains("mariadb") || lower.Contains("dynamodb"))
        {
            return "Database";
        }

        // 7. DevOps & Cloud
        if (lower.Contains("docker") || lower.Contains("kubernetes") || lower.Contains("aws") ||
            lower.Contains("azure") || lower.Contains("cloud") || lower.Contains("git") ||
            lower.Contains("ci/cd") || lower.Contains("devops") || lower.Contains("terraform") ||
            lower.Contains("linux") || lower.Contains("sysadmin"))
        {
            return "DevOps & Cloud";
        }

        // 8. Mobile Development
        if (lower.Contains("flutter") || lower.Contains("react native") || lower.Contains("android") ||
            lower.Contains("ios") || lower.Contains("mobile app"))
        {
            return "Mobile";
        }

        // 9. Testing & QA
        if (lower.Contains("test") || lower.Contains("jest") || lower.Contains("cypress") ||
            lower.Contains("qa") || lower.Contains("selenium") || lower.Contains("xunit") ||
            lower.Contains("quality assurance"))
        {
            return "Testing";
        }

        // 10. Office & Productivity Tools
        if (lower.Contains("microsoft") || lower.Contains("excel") || lower.Contains("word") ||
            lower.Contains("google workspace") || lower.Contains("365") || lower.Contains("computer literacy") ||
            lower.Contains("office management") || lower.Contains("writing reports") || lower.Contains("scheduling"))
        {
            return "Office & Productivity Tools";
        }

        // 11. UI/UX & Creative Media
        if (lower.Contains("adobe") || lower.Contains("figma") || lower.Contains("ui/ux") ||
            lower.Contains("ux design") || lower.Contains("ui design") || lower.Contains("graphic design") ||
            lower.Contains("photoshop") || lower.Contains("illustrator") || lower.Contains("voice acting") ||
            lower.Contains("creative writing") || lower.Contains("design tools"))
        {
            return "UI/UX & Creative Media";
        }

        // 12. Security & Defence
        if (lower.Contains("military") || lower.Contains("security") || lower.Contains("cyber") ||
            lower.Contains("safety") || lower.Contains("liaison") || lower.Contains("protection") ||
            lower.Contains("guard") || lower.Contains("occupational health") || lower.Contains("surveillance"))
        {
            return "Security & Safety";
        }

        // 13. Operations, Logistics & Supply Chain
        if (lower.Contains("warehouse") || lower.Contains("inventory") || lower.Contains("stock control") ||
            lower.Contains("quality control") || lower.Contains("driving record") || lower.Contains("driving") ||
            lower.Contains("logistics") || lower.Contains("supply chain") || lower.Contains("asset management"))
        {
            return "Operations, Logistics & Supply Chain";
        }

        // 14. Construction, Trades & Manufacturing
        if (lower.Contains("metal forming") || lower.Contains("metal framing") || lower.Contains("wood assembly") ||
            lower.Contains("structural drawing") || lower.Contains("construction") || lower.Contains("plumbing") ||
            lower.Contains("welding") || lower.Contains("carpentry") || lower.Contains("electrician") ||
            lower.Contains("masonry") || lower.Contains("machinery") || lower.Contains("maintenance") ||
            lower.Contains("repair") || lower.Contains("mechanic") || lower.Contains("hvac"))
        {
            return "Construction, Trades & Maintenance";
        }

        // 15. Engineering & CAD
        if (lower.Contains("autocad") || lower.Contains("cad") || lower.Contains("drafting") ||
            lower.Contains("solidworks") || lower.Contains("civil engineering") || lower.Contains("electrical engineering") ||
            lower.Contains("structural engineering") || lower.Contains("engineering"))
        {
            return "Engineering & CAD";
        }

        // 16. Business, Finance & Accounting
        if (lower.Contains("accounting") || lower.Contains("financial") || lower.Contains("finance") ||
            lower.Contains("erp") || lower.Contains("tax") || lower.Contains("audit") ||
            lower.Contains("bookkeeping") || lower.Contains("payroll") || lower.Contains("budget") ||
            lower.Contains("sap") || lower.Contains("quickbooks") || lower.Contains("banking"))
        {
            return "Business, Finance & Accounting";
        }

        // 17. Sales, Marketing & Business
        if (lower.Contains("brand management") || lower.Contains("customer relationship") || lower.Contains("market analysis") ||
            lower.Contains("crm") || lower.Contains("salesforce") || lower.Contains("advertising") ||
            lower.Contains("marketing") || lower.Contains("seo") || lower.Contains("social media") ||
            lower.Contains("sales") || lower.Contains("branding") || lower.Contains("lead generation"))
        {
            return "Sales, Marketing & Business";
        }

        // 18. Management, Leadership & HR
        if (lower.Contains("team management") || lower.Contains("supervising") || lower.Contains("supervision") ||
            lower.Contains("management control") || lower.Contains("salary administration") || lower.Contains("productivity management") ||
            lower.Contains("integrity management") || lower.Contains("positive discipline") || lower.Contains("communication with candidates") ||
            lower.Contains("project management") || lower.Contains("agile") || lower.Contains("scrum") ||
            lower.Contains("leadership") || lower.Contains("operations") || lower.Contains("hr") || lower.Contains("recruitment"))
        {
            return "Management, Leadership & HR";
        }

        // 19. Customer Service & Support
        if (lower.Contains("customer service") || lower.Contains("customer support") || lower.Contains("call center") ||
            lower.Contains("helpdesk") || lower.Contains("client relations") || lower.Contains("customer care"))
        {
            return "Customer Service & Support";
        }

        // 20. Education & Healthcare
        if (lower.Contains("childcare") || lower.Contains("early childhood") || lower.Contains("teaching") ||
            lower.Contains("tutor") || lower.Contains("education") || lower.Contains("nursing") ||
            lower.Contains("healthcare") || lower.Contains("medical") || lower.Contains("pedagogy"))
        {
            return "Education & Healthcare";
        }

        // 21. Soft Skills & Interpersonal Traits
        if (lower.Contains("ability to meet deadlines") || lower.Contains("ability to simplify") ||
            lower.Contains("accountability") || lower.Contains("achievement driven") || lower.Contains("achievement oriented") ||
            lower.Contains("action oriented") || lower.Contains("active learning") || lower.Contains("active listening") ||
            lower.Contains("adaptability") || lower.Contains("resilience") || lower.Contains("analytical skills") ||
            lower.Contains("basic reading") || lower.Contains("basic writing") || lower.Contains("communication") ||
            lower.Contains("creative problem-solving") || lower.Contains("creative thinking") || lower.Equals("creativity") ||
            lower.Contains("driven personality") || lower.Contains("group work") || lower.Contains("negotiation") ||
            lower.Contains("organization") || lower.Contains("persuasive communication") || lower.Contains("proactivity") ||
            lower.Contains("problem solving") || lower.Contains("punctuality") || lower.Contains("sociability") ||
            lower.Contains("time management") || lower.Contains("interpersonal") || lower.Contains("work ethic") ||
            lower.Contains("critical thinking") || lower.Contains("collaboration"))
        {
            return "Soft Skills & Interpersonal Traits";
        }

        return "Professional & General Skills";
    }
}
