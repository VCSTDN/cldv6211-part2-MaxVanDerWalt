using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KhumaloCraft.Data;
using KhumaloCraft.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Specialized;

namespace KhumaloCraft.Controllers
{
    public class MyWorkController : Controller
    {
        private readonly KhumaloCraftContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MyWorkController(KhumaloCraftContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: MyWork
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("AdminIndex");
                }
                else
                {
                    return RedirectToAction("UserIndex");

                }
            }
            else
            {
                return RedirectToAction("UserIndex");
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            return View(await _context.MyWorkModel.ToListAsync());
        }

        public async Task<IActionResult> UserIndex()
        {
            return View(await _context.MyWorkModel.ToListAsync());
        }

        // GET: MyWork/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myWorkModel = await _context.MyWorkModel
                .FirstOrDefaultAsync(m => m.WorkID == id);
            if (myWorkModel == null)
            {
                return NotFound();
            }

            return View(myWorkModel);
        }

        // GET: MyWork/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MyWork/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,ProductDescription,Price,InStock,ImagePath,Image,CategoryID")] MyWorkModel myWork)
        {
            if (ModelState.IsValid)
            {
                if (myWork.Image != null)
                {

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images"); // Dedicated folder
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + myWork.Image.FileName;                    
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await myWork.Image.CopyToAsync(stream);
                    }

                    // Use virtual path within application
                    myWork.ImagePath = "/Images/" + uniqueFileName;
                  
                }

                _context.Add(myWork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminIndex));
            }
            ViewData["CategoryID"] = new SelectList(_context.Set<CategoryModel>(), "CategoryID", "CategoryName", myWork.CategoryID);
            return View(myWork);
        }

        // GET: MyWork/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myWorkModel = await _context.MyWorkModel.FindAsync(id);
            if (myWorkModel == null)
            {
                return NotFound();
            }
            return View(myWorkModel);
        }

        // POST: MyWork/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkID,ProductName,ProductDescription,Price,InStock,ImagePath,CategoryID")] MyWorkModel myWork)
        {
            if (id != myWork.WorkID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMyWork = await _context.MyWorkModel.AsNoTracking().FirstOrDefaultAsync(w => w.WorkID == id);
                    if (existingMyWork == null)
                    {
                        return NotFound();
                    }

                    if (myWork.Image != null)
                    {
                        if (!string.IsNullOrEmpty(existingMyWork.ImagePath))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingMyWork.ImagePath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + myWork.Image.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await myWork.Image.CopyToAsync(stream);
                        }
                        myWork.ImagePath = Path.Combine("Images", uniqueFileName);
                    }
                    else
                    {
                        myWork.ImagePath = existingMyWork.ImagePath;
                    }
                    _context.Update(myWork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyWorkModelExists(myWork.WorkID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminIndex));
            }
            return RedirectToAction(nameof(AdminIndex));
        }

        // GET: MyWork/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myWorkModel = await _context.MyWorkModel
                .FirstOrDefaultAsync(m => m.WorkID == id);
            if (myWorkModel == null)
            {
                return NotFound();
            }

            return View(myWorkModel);
        }

        // POST: MyWork/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myWorkModel = await _context.MyWorkModel.FindAsync(id);
            if (myWorkModel != null)
            {
                _context.MyWorkModel.Remove(myWorkModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyWorkModelExists(int id)
        {
            return _context.MyWorkModel.Any(e => e.WorkID == id);
        }
    }
}
