﻿using LiWiMus.Core.Entities;
using LiWiMus.Core.Interfaces;
using LiWiMus.Core.Models;
using LiWiMus.Core.Settings;
using LiWiMus.Core.Specifications;
using LiWiMus.SharedKernel.Interfaces;
using LiWiMus.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LiWiMus.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IRepository<UserPlan> _userPlanRepository;
    private readonly IRepository<Plan> _planRepository;
    private readonly IAvatarService _avatarService;
    private readonly IWebHostEnvironment _environment;
    private readonly IOptions<DataSettings> _dataDirectoriesOptions;
    private readonly IMailService _mailService;
    private readonly IRazorViewRenderer _razorViewRenderer;

    private readonly HttpClient _httpClient = new();

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
                             IRepository<UserPlan> userPlanRepository, IRepository<Plan> planRepository,
                             IAvatarService avatarService, IWebHostEnvironment environment,
                             IOptions<DataSettings> dataDirectoriesOptions, IMailService mailService, 
                             IRazorViewRenderer razorViewRenderer)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userPlanRepository = userPlanRepository;
        _planRepository = planRepository;
        _avatarService = avatarService;
        _environment = environment;
        _dataDirectoriesOptions = dataDirectoriesOptions;
        _mailService = mailService;
        _razorViewRenderer = razorViewRenderer;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var defaultPlan = await _planRepository.GetBySpecAsync(new DefaultPlanSpecification()) ??
                          throw new SystemException();
        var userPlan = new UserPlan
        {
            Plan = defaultPlan,
            Start = DateTime.Now,
            End = DateTime.MaxValue
        };
        await _userPlanRepository.AddAsync(userPlan);
        var user = new User {Email = model.Email, UserName = model.UserName, UserPlan = userPlan};
        await _avatarService.SetRandomAvatarAsync(user, _httpClient, _environment.ContentRootPath,
            _dataDirectoriesOptions.Value.AvatarsDirectory);
        var result = await _userManager.CreateAsync(user, model.Password);
        
        if (result.Succeeded)
        {
            await SendConfirmEmailAsync(user);
            
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
    {
        if (userId == "" || code == "")
        {
            return View("Error");
        }
        
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return View("Error");
        }
        
        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        return View("Error");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    private async Task SendConfirmEmailAsync(User user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        var confirmUrl = Url.Action(
            "ConfirmEmail", 
            "Account", 
            new { userId = user.Id, code = token },
            HttpContext.Request.Scheme);

        var mailRequest = await MailRequest.CreateConfirmEmailAsync(_razorViewRenderer, 
            user.UserName, user.Email, confirmUrl!);
            
        await _mailService.SendEmailAsync(mailRequest);
    } 
}