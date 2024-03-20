using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Repos;
using System;

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

        [HttpGet]
        public ActionResult<IEnumerable<ReservationDTO>> GetReservations()
        {
            var reservations = _reservationRepo.GetAllReservation()
                .Select(r => new ReservationDTO
                {
                    Id = r.Id,
                    UserSsn = r.UserSsn,
                    CarId = r.CarId,
                    StartRentTime = r.StartRentTime,
                    EndRentDate = r.EndRentDate,
                    TotalPrice = r.TotalPrice
                })
                .ToList();

            return Ok(reservations);
        }

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
                Id = reservation.Id,
                UserSsn = reservation.UserSsn,
                CarId = reservation.CarId,
                StartRentTime = reservation.StartRentTime,
                EndRentDate = reservation.EndRentDate,
                TotalPrice = reservation.TotalPrice
            };

            return Ok(reservationDto);
        }

        [HttpPost]
        public ActionResult<ReservationDTO> PostReservation(ReservationDTO reservationDto)
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

        [HttpDelete("{id}")]
        public ActionResult DeleteReservations(int id)
        {
            try
            {
                var reservationToDelete = _reservationRepo.GetReservationById(id);

                if (reservationToDelete == null)
                {
                    return NotFound($"Reservation with Id = {id} not found");
                }

                _reservationRepo.Delete(id);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }

    }
}