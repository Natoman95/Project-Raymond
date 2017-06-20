﻿using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;

namespace Gordon360.Controllers.Api
{

    [RoutePrefix("api/profiles")]
    [CustomExceptionFilter]
    [Authorize]
    public class ProfilesController : ApiController
    {
        private IProfileService _profileService;

        public ProfilesController()
        {
            var _unitOfWork = new UnitOfWork();
            _profileService = new ProfileService(_unitOfWork);
        }

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>Get a single activity based upon the string id entered in the URL</summary>
        /// <param name="username">An identifier for a single activity</param>
        /// <returns></returns>
        /// <remarks>Get a single activity from the database</remarks>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{username}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]
        public IHttpActionResult Get(string username)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(username))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            // search username in three tables
            var student = _profileService.GetStudentProfileByUsername(username);
            var faculty = _profileService.GetFacultyStaffProfileByUsername(username);
            var alumni = _profileService.GetAlumniProfileByUsername(username);

            // merge the person's info if this person is in multiple tables and return result 
            if (student != null)
            {   
                if(faculty != null)
                {
                    if (alumni != null)
                    {
                        JObject stualufac = JObject.FromObject(student);                                 //convert into JSON object in order to use JSON.NET library 
                        stualufac.Merge(JObject.FromObject(alumni), new JsonMergeSettings                // user Merge function to merge two json object
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        });
                        stualufac.Merge(JObject.FromObject(faculty), new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        });
                        stualufac.Add("PersonType", "stualufac");                                          // assign a type to the json object 
                        return Ok(stualufac);
                        //return Json ( new { type = "stualufac", student, faculty, alumni});
                    }
                    JObject stufac = JObject.FromObject(student);
                    stufac.Merge(JObject.FromObject(faculty), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    stufac.Add("PersonType", "stufac");
                    return Ok(stufac);
                }
                else if (alumni != null)
                {
                    JObject stualu = JObject.FromObject(student);
                    stualu.Merge(JObject.FromObject(alumni), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    stualu.Add("PersonType", "stualu");
                    return Ok(stualu);
                    //return Json(new { type = "stualu", student, alumni });
                }
                JObject stu = JObject.FromObject(student);
                stu.Add("PersonType", "stu");
                return Ok(stu);
                //return Json( new { type = "student", student });
            }
            else if (faculty != null)
            {
                if(alumni != null)
                {
                    JObject alufac = JObject.FromObject(alumni);
                    alufac.Merge(JObject.FromObject(faculty), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    alufac.Add("PersonType", "alufac");
                    return Ok(alufac);
                    //return Json(new { type = "alufac", alumni, faculty });
                }
                JObject fac = JObject.FromObject(faculty);
                fac.Add("PersonType", "fac");
                return Ok(fac);
                //return Json(new { type = "faculty", faculty});                
            }
            else if (alumni != null)
            {
                JObject alu = JObject.FromObject(alumni);
                alu.Add("PersonType", "alu");
                return Ok(alu);
                //return Json( new { type = "alumni", alumni });
            }
            else
            {
                return NotFound();
            }
        }
    }
}
