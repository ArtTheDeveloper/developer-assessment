using DataExporter.Dtos;
using DataExporter.Model;
using Microsoft.EntityFrameworkCore;


namespace DataExporter.Services
{
    public class PolicyService
    {
        private readonly ExporterDbContext _dbContext;

        public PolicyService(ExporterDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates a new policy from the DTO.
        /// </summary>
        /// <param name="createPolicyDto">The DTO containing data for the new record</param>
        /// <returns>Returns a ReadPolicyDto representing the new policy, if succeeded. Returns null, otherwise.</returns>
        public async Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto createPolicyDto)
        {
            if (createPolicyDto == null)
                return null;

            var policy = new Policy
            {
                PolicyNumber = createPolicyDto.PolicyNumber,
                Premium = createPolicyDto.Premium,
                StartDate = createPolicyDto.StartDate
            };

            _dbContext.Policies.Add(policy);

            await _dbContext.SaveChangesAsync();

            return new ReadPolicyDto
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            };
        }

        /// <summary>
        /// Retrives all policies.
        /// </summary>
        /// <returns>Returns a list of ReadPolicyDto.</returns>
        public async Task<IList<ReadPolicyDto>> ReadPoliciesAsync()
        {
            return await _dbContext.Policies
                                   .AsNoTracking()
                                   .Select(p => new ReadPolicyDto
                                   {
                                       Id = p.Id,
                                       PolicyNumber = p.PolicyNumber,
                                       Premium = p.Premium,
                                       StartDate = p.StartDate
                                   }).ToListAsync();
        }

        /// <summary>
        /// Retrieves a policy by id.
        /// </summary>
        /// <param name="id">The Id of the record to retrieve</param>
        /// <returns>Returns a ReadPolicyDto if found, null otherwise.</returns>
        public async Task<ReadPolicyDto?> ReadPolicyAsync(int id)
        {
            var policy = await _dbContext.Policies.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);

            if (policy == null) 
                return null;

            return new ReadPolicyDto
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            };
        }

        /// <summary>
        /// Retrieves policies within a date range and maps them to a List of ExportDto.
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>A list of policies with notes filtered by dates</returns>
        public async Task<IList<ExportDto>> ExportPoliciesAsync(DateTime startDate, DateTime endDate)
        {
            var policies = await _dbContext.Policies
                .Where(p => p.StartDate >= startDate && p.StartDate <= endDate)
                .Include(p => p.Notes)
                .ToListAsync();

            return policies.Select(p => new ExportDto
            {
                PolicyNumber = p.PolicyNumber,
                Premium = p.Premium,
                StartDate = p.StartDate,
                Notes = p.Notes.Select(n => n.Text).ToList()
            }).ToList();
        }
    }
}
