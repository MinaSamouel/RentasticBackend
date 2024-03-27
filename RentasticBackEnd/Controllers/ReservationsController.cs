using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RentasticBackEnd.Models;

namespace RentasticBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepo _reservationRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepo _userRepo;

        public ReservationsController(IReservationRepo reservationRepo, UserManager<ApplicationUser> userManager, IUserRepo userRepo)
        {
            _reservationRepo = reservationRepo;
            _userManager = userManager;
            _userRepo = userRepo;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Reservation>> GetReservations()
        {
            var reservations = _reservationRepo.GetAllReservations();

            return Ok(reservations);
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Reservation> GetReservation(int id)
        {
            var reservation = _reservationRepo.GetReservationById(id);

            if (reservation == null)
            {
                return NotFound();
            }

            //var reservationDto = new ReservationDTO
            //{
            //    UserSsn = reservation.UserSsn,
            //    CarId = reservation.CarId,
            //    StartRentTime = reservation.StartRentTime,
            //    EndRentDate = reservation.EndRentDate,
            //    TotalPrice = reservation.TotalPrice
            //};

            return Ok(reservation);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> PostReservation([FromBody] ReservationDTO reservationDto)
        {
            var userLogin = await _userManager.FindByIdAsync(reservationDto.UserGuid);
            if (userLogin == null)
            {
                return Unauthorized("UserNoteAllowed");
            }

            var checkUser = _userRepo.GetOneByEmail(userLogin.Email!);
            var reservation = new Reservation
            {
                UserSsn = checkUser!.Ssn,
                CarId = reservationDto.CarId,
                StartRentTime = reservationDto.StartRentTime,
                EndRentDate = reservationDto.EndRentDate,
                TotalPrice = reservationDto.TotalPrice
            };

            _reservationRepo.Add(reservation);

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservationDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteReservation(int id)
        {
            var reservationToDelete = _reservationRepo.GetReservationById(id);

            if (reservationToDelete == null)
            {
                return NotFound($"Reservation with Id = {id} not found");
            }

            _reservationRepo.Delete(id);
            return NoContent();
        }
    }
}
