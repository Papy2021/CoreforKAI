using Core.Modeles;
using Core.ViewModeles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Core.Controllers
{
    [Authorize(Roles = "Admin,Super Admin")]
    public class AdministrationController : Controller
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS8602 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference return.
#pragma warning disable CS8620 // Possible null reference return.

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;



        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager,
            ILogger<AdministrationController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userRole = new IdentityRole() {Name=model.RoleName };
                var result = await _roleManager.CreateAsync(userRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                
            }
            return View();
        }


        [HttpGet]
        public IActionResult ListRoles()
        {
           var roles= _roleManager.Roles.ToList();
            return View(roles);
        }




        [Authorize(Policy = "EditRolePolicy")]
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role= await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                if (id.IsNullOrEmpty())
                {
                    ViewBag.Message = $"There's no role without an id on the system";
                }
                ViewBag.Message = $"There's no role with id: {id} on the system";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
            };
            foreach (var user in _userManager.Users.ToList())
            {
               if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View (model);
        }




        [Authorize(Policy = "EditRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                if (model.Id.IsNullOrEmpty())
                {
                    ViewBag.Message = $"There's no role without an id on the system";
                }
                ViewBag.Message = $"There's no role with id: {model.Id} on the system";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var resultUpdate=await _roleManager.UpdateAsync(role);
                if(resultUpdate.Succeeded)
                {
                  return  RedirectToAction("listroles", "administration");
                }
                else
                {
                    foreach(var error in resultUpdate.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
         
        }



        [Authorize(Policy = "EditRolePolicy")]
        [Authorize(Policy = "DeleteRolePolicy")]
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId=roleId;
            ViewBag.RoleName = await _roleManager.FindByIdAsync(roleId);
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                if (roleId.IsNullOrEmpty())
                {
                    ViewBag.Message = $"There's no role without an id on the system";
                }
                ViewBag.Message = $"There's no role with id: {roleId} on the system";
                return View("NotFound");
            }

            List<EditUsersInRoleViewModel> UsersList = new();


            var AllUsers = _userManager.Users.ToList();
            foreach (var user in AllUsers)
            {

                EditUsersInRoleViewModel user_role = new()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    user_role.IsSelected = true;
                }
                else
                {
                    user_role.IsSelected = false;
                }
                UsersList.Add(user_role);
            }
            return View(UsersList);
        }


        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> EditUsersInRole(List<EditUsersInRoleViewModel> model, string roleId)
        {

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
        
                ViewBag.Message = $"There's no role with id: {roleId} on the system";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                var result = new IdentityResult();
                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < model.Count - 1)
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { id = roleId });
                    }

                }
            }
            return RedirectToAction("EditRole", new { id = roleId });
        }


        [HttpGet]
        public IActionResult ListUsers()
        {

            var allUsers =  _userManager.Users.ToList();
            return View(allUsers);
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
           var currentUser= await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                ViewBag.Message = $"There's no user with id: {id} on the system";
                return View("NotFound");
            }
            else
            {
                var userClaims=await _userManager.GetClaimsAsync(currentUser);
                var userRoles=await _userManager.GetRolesAsync(currentUser);

                var updateUser = new EditUserViewModel
                {
                    Id = currentUser.Id,
                    UserName = currentUser.UserName,
                    Email = currentUser.Email,
                    Claims=userClaims.Select(c =>c.Type+" : "+c.Value).ToList(),
                    Roles=userRoles.ToList(),
                };
                return View(updateUser);
            }
           
        }


        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var currentUser = await _userManager.FindByIdAsync(model.Id);
            if (currentUser == null)
            {
                ViewBag.Message = $"There's no user with id: {model.Id} on the system";
                return View("NotFound");
            }
            else
            {
                currentUser.Email = model.Email;
                currentUser.UserName = model.UserName;
                var result = await _userManager.UpdateAsync(currentUser);

                if (result.Succeeded)
                {
                  return RedirectToAction("ListUsers", "Administration");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View(model);
                    }
                }
                return View(model);
            }
        }


        [HttpPost]
        public async  Task<IActionResult> DeleteUser(string id)
        {
            var currentUser=await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                ViewBag.Message = $"There's no user with id: {id} on the system";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await _userManager.DeleteAsync(currentUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return View("ListUsers");
                }
                catch (DbUpdateException ex)
                {
                    

                    _logger.LogError(ex, ex.Message,string.Empty);
                    ViewBag.ErrorTitle = $"The user {currentUser.UserName} cannot be deleted";
                    ViewBag.ErrorMessage = $"To be abble to delete {currentUser.UserName} please remove him/her from every roles et Claims";
                    return View("Error");
                }
            }  
        }



        [Authorize(Policy = "EditRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var currentRole = await _roleManager.FindByIdAsync(id);
            if (currentRole == null)
            {
                ViewBag.Message = $"There's no role with id: {id} on the system";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await _roleManager.DeleteAsync(currentRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles", "Administration");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"Exception Occured: {ex}");
                    ViewBag.ErrorTitle = $"{currentRole.Name} role cannot be deleted";
                    ViewBag.ErrorMessage = $"To be abble to delete {currentRole.Name} please first remove all the users in this role";
                    return View("Error");
                }

            }
        }
        


        [HttpGet]
        [Authorize(Policy = "EditUserRolesClaims")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                ViewBag.Message = $"There's no user with id: {userId} on the system";
                return View("NotFound");
            }

            List<UserRolesViewModel> rolesList = new();
            var allRoles = _roleManager.Roles.ToList();
            foreach (var role in allRoles)
            {
                UserRolesViewModel userRole = new()
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                };

                if (await _userManager.IsInRoleAsync(currentUser, role.Name))
                {
                    userRole.IsSelected = true;
                }
                else
                {
                    userRole.IsSelected = false;
                }
                rolesList.Add(userRole);
            }
            return View(rolesList);
        }




        [Authorize(Policy = "EditUserRolesClaims")]
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model,string userId)
        {
            ViewBag.userId = userId;
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                ViewBag.Message = $"There's no user with id: {userId} on the system";
                return View("NotFound");
            }


            
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var result= await _userManager.RemoveFromRolesAsync(currentUser, userRoles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot remove user existing roles");
                return View(model);
            }

            var newRoles = model.Where(x => x.IsSelected).Select(y => y.RoleName);
            result = await _userManager.AddToRolesAsync(currentUser, newRoles);

           if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { id = userId });
        }


        [Authorize(Policy = "EditUserRolesClaims")]
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                ViewBag.Message = $"There's no user with id: {userId} on the system";
                return View("NotFound");
            }

            var model = new UserClaimsViewModel { UserId = userId };
            var currentUserClaims= await _userManager.GetClaimsAsync(currentUser);

            foreach (var claim in ClaimsStore.AllClaims) 
            {
                UserClaim userClaim = new() { ClaimType = claim.Type };
                if(currentUserClaims.Any(c=>c.Type == claim.Type && c.Value=="True"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }
            return View(model);  
        }




        [Authorize(Policy = "EditUserRolesClaims")]
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var currentUser = await _userManager.FindByIdAsync(model.UserId);
            if (currentUser == null)
            {
                ViewBag.Message = $"There's no user with id: {model.UserId} on the system";
                return View("NotFound");
            }

            var currentUserClaims = await _userManager.GetClaimsAsync(currentUser);
            var result = await _userManager.RemoveClaimsAsync(currentUser, currentUserClaims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            var selectedClaims = model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "True": "False"));
            result = await _userManager.AddClaimsAsync(currentUser, selectedClaims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new {id=model.UserId});
        }
    }
}
