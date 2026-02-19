using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly InventoryManagementDbContext _context;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(InventoryManagementDbContext context, IDepartmentRepository departmentRepository, IDepartmentService departmentService)
        {
            _context = context;
            _departmentRepository = departmentRepository;
            _departmentService = departmentService;
        }

        // GET: Departments
        public async Task<IActionResult> Index()

        
        {
            //var s = _departmentService.
            return View(await _departmentRepository.GetAllAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(DepartmentDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _departmentService.CreateDepartmentAsync(model);
                var department = await _context.Departments
                          .FirstOrDefaultAsync(d => d.DepDesc == model.DepDesc);
                _context.Departments.Add(new Department());
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Oracle-friendly error handling
                ModelState.AddModelError("", "Unable to save data. " + ex.Message);
                return View(model);
            }
        }
        public IActionResult Edit(int id)
        {
            var department = _context.Departments.Find(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Department model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _context.Departments.Update(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Update failed: " + ex.Message);
                return View(model);
            }
        }
        public IActionResult Delete(int id)
        {
            var department = _context.Departments.Find(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int DepCode)
        {
            try
            {
                var department = _context.Departments.Find(DepCode);
                if (department == null)
                    return NotFound();

                _context.Departments.Remove(department);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Delete failed: " + ex.Message);
                return View();
            }
        }

    }
}
