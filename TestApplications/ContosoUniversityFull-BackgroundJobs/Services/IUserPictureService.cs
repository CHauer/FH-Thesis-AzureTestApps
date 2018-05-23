﻿using ContosoUniversityFull.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContosoUniversityFull.Services
{
    public interface IUserPictureService
    {
        Task GenerateUserPicture(int pictureId);
    }
}