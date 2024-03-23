using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using RentasticBackEnd.Models;

namespace RentasticBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepo _reservationRepo;

        public ReservationsController(IReservationRepo reservationRepo)
        {
            _reservationRepo = reservationRepo;
        }
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<ReservationDTO>> GetReservations()
        {
            var reservations = _reservationRepo.GetAllReservations()
                .Select(r => new ReservationDTO
                {
                    UserSsn = r.UserSsn,
                    CarId = r.CarId,
                    StartRentTime = r.StartRentTime,
                    EndRentDate = r.EndRentDate,
                    TotalPrice = r.TotalPrice
                }).ToList();

            return Ok(reservations);
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<ReservationDTO> GetReservation(int id)
        {
            var reservation = _reservationRepo.GetReservationById(id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationDto = new ReservationDTO
            {
                UserSsn = reservation.UserSsn,
                CarId = reservation.CarId,
                StartRentTime = reservation.StartRentTime,
                EndRentDate = reservation.EndRentDate,
                TotalPrice = reservation.TotalPrice
            };

            return Ok(reservationDto);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult<ReservationDTO> PostReservation([FromBody] ReservationDTO reservationDto)
        {
            var reservation = new Reservation
            {
                UserSsn = reservationDto.UserSsn,
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
