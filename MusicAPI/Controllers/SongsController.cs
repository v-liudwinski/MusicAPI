﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAPI.Data;
using MusicAPI.Helpers;
using MusicAPI.Models;

namespace MusicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;
        
        public SongsController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Song song)
        {
            string imageUrl = await FileHelper.UploadImage(song.Image);
            song.ImageUrl = imageUrl;
            string audioUrl = await FileHelper.UploadFile(song.Audio);
            song.AudioUrl = audioUrl;
            song.UploadedDate = DateTime.Now;
            await _dbContext.Songs.AddAsync(song);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongs(int? pageNumber, int? pageSize)
        {
            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 5;
            var songs = await (from song in _dbContext.Songs
                select new
                {
                    Id = song.Id,
                    Title = song.Title,
                    Duration = song.Duration,
                    ImageUrl = song.ImageUrl,
                    AudioUrl = song.AudioUrl
                }).ToListAsync();
            return Ok(songs.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
        }
        
        [HttpGet("[action]")]
        public async Task<IActionResult> FeaturedSongs()
        {
            var songs = await (from song in _dbContext.Songs
                where song.IsFeatured == true
                select new
                {
                    Id = song.Id,
                    Title = song.Title,
                    Duration = song.Duration,
                    ImageUrl = song.ImageUrl,
                    AudioUrl = song.AudioUrl
                }).ToListAsync();
            return Ok(songs);
        }
        
        [HttpGet("[action]")]
        public async Task<IActionResult> NewSongs()
        {
            var songs = await (from song in _dbContext.Songs
                orderby song.UploadedDate descending
                select new
                {
                    Id = song.Id,
                    Title = song.Title,
                    Duration = song.Duration,
                    ImageUrl = song.ImageUrl,
                    AudioUrl = song.AudioUrl
                }).Take(15).ToListAsync();
            return Ok(songs);
        }
        
        [HttpGet("[action]")]
        public async Task<IActionResult> SearchSongs(string query)
        {
            /*var songs = await (from song in _dbContext.Songs
                where song.Title.StartsWith(query)
                select new
                {
                    Id = song.Id,
                    Title = song.Title,
                    Duration = song.Duration,
                    ImageUrl = song.ImageUrl,
                    AudioUrl = song.AudioUrl
                }).Take(15).ToListAsync();*/
            var songs = await _dbContext.Songs
                .Where(s => s.Title.StartsWith(query))
                .Select(s => new
                {
                    Id = s.Id,
                    Title = s.Title,
                    Duration = s.Duration,
                    ImageUrl = s.ImageUrl,
                    AudioUrl = s.AudioUrl
                })
                .Take(15)
                .ToListAsync();
            return Ok(songs);
        }
    }
}