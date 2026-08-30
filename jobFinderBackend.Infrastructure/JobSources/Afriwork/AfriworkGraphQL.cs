namespace jobFinder.Infrastructure.JobSources.Afriwork;

public static class AfriworkGraphQL
{
    public const string GetAllJobs = """
        query GetAllJobs(
          $offset: Int!
          $limit: Int!
          $whereCondition: jobs_bool_exp!
          $orderCondition: [jobs_order_by!]
        ) {
          jobs(
            order_by: $orderCondition
            offset: $offset
            limit: $limit
            where: $whereCondition
          ) {
            id
            title
            created_at
            updated_at
            published_at
            refreshed_at
            approval_status
            description
            job_type
            job_site

            skill_requirements {
              skill {
                name
                id
              }
            }

            city {
              name
              country {
                name
              }
            }

            sectors {
              sector {
                name
                id
              }
            }

            deadline
            compensation_amount_cents
            compensation_type
            compensation_currency
            experience_level

            entity {
              type
              name
            }
          }
        }
        """;
}