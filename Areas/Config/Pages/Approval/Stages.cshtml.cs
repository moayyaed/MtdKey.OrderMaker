/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MtdKey.OrderMaker.Entity;

namespace MtdKey.OrderMaker.Areas.Config.Pages.Approval
{
    public class StagesModel : PageModel
    {

        private readonly DataConnector _context;

        public StagesModel(DataConnector context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdApproval MtdApproval { get; set; }                
        public IList<MtdApprovalStage> Stages { get; set; }
        public async Task<IActionResult> OnGetAsync(string idApproval)
        {

            MtdApproval = await _context.MtdApproval
                .FirstOrDefaultAsync(x => x.Id == idApproval);

            if (MtdApproval == null)
            {
                return NotFound();
            }

            await _context.Entry(MtdApproval).Collection(x=>x.MtdApprovalStages).LoadAsync();
            Stages = await _context.MtdApprovalStage.Where(x => x.MtdApproval == MtdApproval.Id).OrderBy(x => x.Stage).ToListAsync();

            return Page();
        }
    }
}