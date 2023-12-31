﻿using Croissant_Rouge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Croissant_Rouge.Utility;

namespace Croissant_Rouge.Controllers;

public class DonationsController : Controller

{
    private readonly MyContext _context;
    public DonationsController(MyContext context)
    {
        _context = context;
    }

    //********************************************************************************* All Donations Filtred by Category For Admin

    [HttpGet("Donations")]
    public IActionResult AllDonations()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LoginUser", "Users");
        }

        List<Money> userDonations = _context.Moneys
            .ToList();
        int totalAmountDonated = userDonations.Sum(m => m.Amount);
        ViewBag.TotalAmountDonated = totalAmountDonated;

        List<Donation> AllDonations = _context.Donations
            .Include(donnation => donnation.Donner)
            .ToList();



        List<User> AllUsersRole = _context.Users
            .ToList();

        List<Shipment> AllShippers = _context.Shipments
        .Include(shipment => shipment.Shipper)
        .ToList();

        MoneyandDonation br = new MoneyandDonation();
        br.Alldonations = AllDonations;
        br.Allusersrole = AllUsersRole;
        br.AllMoney = userDonations;

        return View(br);
    }


    //******************************************************************************************************************************

    [HttpGet("/donate")]
    public IActionResult Donate()
    {
     if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LoginUser","Users");
        }
     return View();
    }


    //********************************************************************************* Show One Donation For Admin

    [HttpGet("donations/{donationId}")]
    public IActionResult OneDonation(int donationId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LoginUser", "Users");
        }
        Donation? OneDonation = _context.Donations
            .Include(donation => donation.Donner)
            .FirstOrDefault(donation => donation.DonationId == donationId);
        return View(OneDonation);
    }

    //********************************************************************************* Delete Donation For Admin
    [HttpPost("donation/destroy")]
    public IActionResult DeleteDonation(int donationId)
    {
        Donation? DonationToDelete = _context.Donations.FirstOrDefault(s => s.DonationId == donationId);
        _context.Donations.Remove(DonationToDelete);
        _context.SaveChanges();
        return RedirectToAction("AllDonations");
    }

    //********************************************************************************* Valid Donation For Admin

    [HttpPost("donation/{donationId}/validate")]
    public IActionResult ValidateDonation(int donationId)
    {
        Donation? OneDonation = _context.Donations
            .FirstOrDefault(donation => donation.DonationId == donationId);
        OneDonation.status = StaticData.Status.Valid;
        _context.SaveChanges();
        return RedirectToAction("AllDonations");
    }




    //******************************************************* CREATE DONATION *****************************

    [HttpPost("Donation/create")]
    public IActionResult CreateDonation(Donation newDonation, IFormFile imageFile)
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
                    newDonation.Picture = "/uploads/" + fileName;
                }
                else
                {
                    ModelState.AddModelError("imageFile", "Invalid image format or size.");
                    Console.WriteLine($"ERRORS---- {ModelState}");
                    return View("Donate");
                }
            }

            _context.Add(newDonation);
            _context.SaveChanges();
            return RedirectToAction("MyProfile","Users", new { DonationId = newDonation.DonationId });
        }

        return View("Donate");
    }

    private bool IsImageValid(IFormFile imageFile)
    {

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
        return allowedExtensions.Contains(fileExtension);
    }



}