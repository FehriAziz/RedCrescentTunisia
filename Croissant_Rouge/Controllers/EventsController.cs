using Croissant_Rouge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Croissant_Rouge.Utility;

namespace Croissant_Rouge.Controllers;

public class EventsController : Controller

{
    private readonly MyContext _context;
    public EventsController(MyContext context)
    {
        _context = context;

    }




    //******************************************************* CREATE EVENT *****************************

    [HttpGet("Event/create")]
    public IActionResult CreateEvent()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("");
        }
        return View();
    }



    [HttpPost("Event/create")]
    public IActionResult CreateEvent(Event newEvent, IFormFile imageFile)
    {

        if (ModelState.IsValid)
        //    Console.WriteLine($"FORM---- {newDonation}");
        //Console.WriteLine($"IMAGE---- {imageFile}");
        {

            if (imageFile != null)
            {
                // Check the file format, size, or perform any other validation you need.
                if (IsImageValid(imageFile))
                {
                    // Generate a unique file name to avoid conflicts.
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Define the path where you want to save the uploaded image. Make sure this directory exists.
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"); // The path to the "uploads" folder.

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullPath = Path.Combine(path, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    // Save the image file path to the newDonation object.
                    newEvent.Picture = "/uploads/" + fileName;
                }
                else
                {
                    ModelState.AddModelError("imageFile", "Invalid image format or size.");
                    Console.WriteLine($"ERRORS---- {ModelState}");
                    return View("CreatEvent");
                }
            }

            _context.Add(newEvent);
            _context.SaveChanges();
            return RedirectToAction("AllEvents", "Events", new { EventId = newEvent.EventId });
        }

        return View("CreateEvent");
    }

    private bool IsImageValid(IFormFile imageFile)
    {

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
        return allowedExtensions.Contains(fileExtension);
    }


    //******************************************************* ALL EVENT *****************************


    [HttpGet("Events")]
    public IActionResult AllEvents()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LoginUser", "Users");
        }
        int? userId = (int)HttpContext.Session.GetInt32("userId");

        List<Event> AllEvents = _context.Events
            .Include(e => e.EventParticipations)
            .ToList();

        
        User user = _context.Users
            .FirstOrDefault(u => u.UserId == userId);

        UserAndEvent UE = new UserAndEvent();
        UE.Allevents = AllEvents;
        UE.User = user;

        return View(UE);
    }



    //********************************************************************************* Delete Event
    [HttpPost("event/destroy")]
    public IActionResult Deleteevent(int eventId)
    {
        Event? EventToDelete = _context.Events.FirstOrDefault(s => s.EventId == eventId);
        _context.Events.Remove(EventToDelete);
        _context.SaveChanges();
        return RedirectToAction("AllEvents");
    }


    //** Participate to Event

    [HttpPost("participation/create")]
    public IActionResult Participation(Participation newParticipation)
    {
        if (ModelState.IsValid)
        {
            _context.Add(newParticipation);
            _context.SaveChanges();
            return RedirectToAction("AllEvents");
        }
        return RedirectToAction("AllEvents");
    }

    //** Delete Participation

    [HttpPost("participation/destroy")]
    public IActionResult DeleteParticipation(Participation participationToDelete)
    {
        if (ModelState.IsValid)
        {
            _context.Remove(participationToDelete);
            _context.SaveChanges();
            return RedirectToAction("AllEvents");
        }
        return RedirectToAction("AllEvents");
    }






}