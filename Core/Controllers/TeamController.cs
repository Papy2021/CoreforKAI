using Core.Modeles;
using Core.ViewModeles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{

   
    public class TeamController:Controller
    {
        public readonly ITeamMembersRepository _TeamMemberModele;
        public readonly IWebHostEnvironment _Webenvironment;

        public TeamController(ITeamMembersRepository modeles, IWebHostEnvironment Webenvironment)
        {
            _TeamMemberModele = modeles;
            _Webenvironment = Webenvironment;
        }

        public ViewResult Details(int id)
        {
            TeamMember memberInfo = _TeamMemberModele.GetMember(id);
            if (memberInfo == null)
            {
                ViewBag.Message = $"There is no member with id: {id} on the system";
                ViewBag.Title = "Not Found";
                return View("NotFound");
                
            }

            TeamDetailsViewModel modelMember = new()
            {
                Member = memberInfo,
                PageTitle = "Team Member Details"

            };

            return View(modelMember);

        }


        [AllowAnonymous]
        public ViewResult Index()
        {
            TeamIndexViewModel listMember = new()
            {
                ListTeamMembers = _TeamMemberModele.GetAllMembers(),
                PageTitle = "List Of Members",
            };

            return View(listMember);
        }


        [HttpGet]
        public IActionResult Create()
        {
         return View();
        }


        [HttpPost]
        public IActionResult Create(CreateMemberViewModel model)
        {
            
            string uniqueString = "";
            if(ModelState.IsValid)
            {

                if(model.Photo!=null)
                {
                    uniqueString = SavePhoto(model);
                }

                TeamMember newMember = new()
                {
                    Name = model.Name,
                    Position = model.Position,
                    Country = model.Country,
                    Phone = model.Phone,
                    Summary = model.Summary,
                    Nationality = model.Nationality,
                    PhotoPath = uniqueString
                };

                _TeamMemberModele.Add(newMember);
                return RedirectToAction("details", new { id = newMember.Id });
            }
            else
            {
                //return RedirectToAction("list");
               return View(model);
            }
         
           //return View();
        }

        private string SavePhoto(CreateMemberViewModel model)
        {
            string uniqueString;
            string folderName = Path.Combine(_Webenvironment.WebRootPath, "images");
            uniqueString = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(folderName, uniqueString);
           
            using(var fileStreamed = new FileStream(filePath, FileMode.Create))
            {
                model.Photo.CopyTo(fileStreamed);
            }
            

            return uniqueString;
        }


        [HttpGet]
        public ViewResult Edit(int id)
        {
            TeamMember editedMember = _TeamMemberModele.GetMember(id);
            EditMemberViewModel toEditMember = new()
            {
                //Id = employee.Id.Value,
                Name = editedMember.Name,
                Position = editedMember.Position,
                Country = editedMember.Country,
                Phone= editedMember.Phone,
                Summary = editedMember.Summary,
                Nationality = editedMember.Nationality,
                CurrentPhotoPath = editedMember.PhotoPath,


            };
           
            return View(toEditMember); 
        }



        [HttpPost]
        public IActionResult Edit(EditMemberViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                TeamMember updatedMember = _TeamMemberModele.GetMember(model.Id);
                updatedMember.Name = model.Name;
                updatedMember.Position = model.Position;
                updatedMember.Country = model.Country;
                updatedMember.Phone = model.Phone;
                updatedMember.Summary = model.Summary;
                updatedMember.Nationality = model.Nationality;
                if (model.Photo != null)
                {
                    if(model.CurrentPhotoPath != null)
                    {
                        string currentPath= Path.Combine(_Webenvironment.WebRootPath, "images", model.CurrentPhotoPath);
                        System.IO.File.Delete(currentPath);
                    }
                    updatedMember.PhotoPath = SavePhoto(model);
                }
                _TeamMemberModele.Update(updatedMember);
                return RedirectToAction("index");
            }
            else
            {
                return View(model);
            }
        }


    }
}
