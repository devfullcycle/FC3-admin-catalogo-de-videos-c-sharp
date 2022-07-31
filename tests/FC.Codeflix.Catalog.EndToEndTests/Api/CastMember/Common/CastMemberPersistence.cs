using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
public class CastMemberPersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public CastMemberPersistence(CodeflixCatalogDbContext context)
        => _context = context;

    public async Task InsertList(List<DomainEntity.CastMember> castMember)
    {
        await _context.AddRangeAsync(castMember);
        await _context.SaveChangesAsync();
    }

    public async Task<DomainEntity.CastMember?> GetById(Guid id)
        => await _context.CastMembers.AsNoTracking()
            .FirstOrDefaultAsync(castMember => castMember.Id == id);

}
