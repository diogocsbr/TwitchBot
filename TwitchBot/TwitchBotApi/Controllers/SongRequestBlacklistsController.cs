﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TwitchBotDb.Models;

namespace TwitchBotApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class SongRequestBlacklistsController : ControllerBase
    {
        private readonly TwitchBotDbContext _context;

        public SongRequestBlacklistsController(TwitchBotDbContext context)
        {
            _context = context;
        }

        // GET: api/songrequestblacklists/get/2
        // GET: api/songrequestblacklists/get/2?id=1
        [HttpGet("{broadcasterId:int}")]
        public async Task<IActionResult> Get([FromRoute] int broadcasterId, [FromQuery] int id = 0)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var songRequestBlacklist = new object();

            if (id > 0)
                songRequestBlacklist = await _context.SongRequestBlacklist.SingleOrDefaultAsync(m => m.Broadcaster == broadcasterId && m.Id == id);
            else
                songRequestBlacklist = await _context.SongRequestBlacklist.Where(m => m.Broadcaster == broadcasterId).ToListAsync();

            if (songRequestBlacklist == null)
            {
                return NotFound();
            }

            return Ok(songRequestBlacklist);
        }

        // PUT: api/songrequestblacklists/update/2?id=1
        // Body (JSON): 
        [HttpPut("{broadcasterId:int}")]
        public async Task<IActionResult> Update([FromRoute] int broadcasterId, [FromQuery] int id, [FromBody] SongRequestBlacklist songRequestBlacklist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != songRequestBlacklist.Id && broadcasterId != songRequestBlacklist.Broadcaster)
            {
                return BadRequest();
            }

            _context.Entry(songRequestBlacklist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongRequestBlacklistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(songRequestBlacklist);
        }

        // POST: api/songrequestblacklists/create
        // Body (JSON): { "artist": "some stupid artist", "broadcaster": 2 }
        // Body (JSON): { "artist": "some stupid artist", "title": "some stupid title", "broadcaster": 2 }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SongRequestBlacklist songRequestBlacklist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SongRequestBlacklist.Add(songRequestBlacklist);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { broadcasterId = songRequestBlacklist.Broadcaster, id = songRequestBlacklist.Id }, songRequestBlacklist);
        }

        // DELETE: api/songrequestblacklists/delete/2
        // DELETE: api/songrequestblacklists/delete/2?id=1
        [HttpDelete("{broadcasterId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int broadcasterId, [FromQuery] int id = 0)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var songRequestBlacklist = new object();

            if (id > 0)
            {
                SongRequestBlacklist songRequestBlacklistItem = await _context.SongRequestBlacklist.SingleOrDefaultAsync(m => m.Broadcaster == broadcasterId && m.Id == id);

                if (songRequestBlacklistItem == null)
                    return NotFound();

                _context.SongRequestBlacklist.Remove(songRequestBlacklistItem);

                songRequestBlacklist = songRequestBlacklistItem;
            }
            else
            {
                List<SongRequestBlacklist> songRequestBlacklistItems = await _context.SongRequestBlacklist.Where(m => m.Broadcaster == broadcasterId).ToListAsync();

                if (songRequestBlacklistItems == null)
                    return NotFound();

                _context.SongRequestBlacklist.RemoveRange(songRequestBlacklistItems);

                songRequestBlacklist = songRequestBlacklistItems;
            }

            await _context.SaveChangesAsync();

            return Ok(songRequestBlacklist);
        }

        private bool SongRequestBlacklistExists(int id)
        {
            return _context.SongRequestBlacklist.Any(e => e.Id == id);
        }
    }
}