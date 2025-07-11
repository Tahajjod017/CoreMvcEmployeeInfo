﻿using EmployeeMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EmployeeMvc.Controllers
{
    public class EmployeeInfoController1 : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private const int PageSize = 10;
        public EmployeeInfoController1(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index(int page = 1, string search = "")
        // Get Department and Designation for dropdown

        {
            ViewBag.Departments = await _dbContext.Departments.ToListAsync();
            ViewBag.Designation = await _dbContext.Designations.ToListAsync();

           // Query empliyess with search functionality
            var query1 = from emp in _dbContext.Employeeinfos
                         join des in _dbContext.Designations on emp.Designation equals des.DesignationId
                         join dep in _dbContext.Departments on emp.Department equals dep.DepartmentId
                         select new
                         {
                             emp.EmployeeID,
                             emp.Name,
                             emp.Email,
                             emp.Phone,
                             emp.GrossSalary,
                             emp.JoiningDate,
                             Designation = des.DesignationName,
                             Department = dep.DepartmentName
                         };

            var query = query1.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search)
                || e.Email.Contains(search)
                || e.Phone.Contains(search));
            }
            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            var employee = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.viewpage = totalPages;
            ViewBag.viewpage = totalPages;
            ViewBag.viewrecords = totalRecords;
            ViewBag.viewsize = PageSize;
            ViewBag.viewsearch = search;
            ViewBag.Hasprevious = page < 1;
            ViewBag.Hasforward = page < totalPages;
            ViewBag.emp = employee;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Save(Employeeinfo employee)
        {
            if (ModelState.IsValid)
            {
                if (employee.AutoID == 0)
                    _dbContext.Employeeinfos.Add(employee);
                else
                {
                    _dbContext.Employeeinfos.Update(employee);
                }
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
            



        }
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _dbContext.Employeeinfos.FindAsync(id);
            if (employee == null)
            {
                return NotFound();

            }
            return Json(employee);

        }
        public async Task<IActionResult> Clear (int id)
        {
            return (RedirectToAction("Index"));
        }

        public IActionResult AddPartial()
        {
            return PartialView("_AddPartial", new Designation());
            
        }
        

    }
}