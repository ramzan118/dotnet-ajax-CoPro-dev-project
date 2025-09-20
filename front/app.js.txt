const API = 'http://localhost:5000/api';

async function fetchSlots() {
  const res = await fetch(`${API}/slots`);
  if (!res.ok) return showMsg('Unable to load slots', true);
  const slots = await res.json();
  renderSlots(slots);
}

function renderSlots(slots) {
  const container = document.getElementById('slots');
  container.innerHTML = '';
  if (slots.length === 0) {
    container.innerText = 'No slots configured.';
    return;
  }
  slots.forEach(s => {
    const div = document.createElement('div');
    div.className = 'slot';
    div.innerHTML = `
      <strong>${s.startTime} — ${s.endTime}</strong>
      <div>Capacity: ${s.capacity}</div>
      <div>Booked: ${s.bookedCount}</div>
      <div>Status: ${s.isAvailable ? '<span style="color:green">Available</span>' : '<span style="color:red">Full</span>'}</div>
    `;
    const btn = document.createElement('button');
    btn.textContent = 'Book';
    btn.disabled = !s.isAvailable;
    btn.onclick = () => openBookingForm(s);
    div.appendChild(btn);
    container.appendChild(div);
  });
}

function openBookingForm(slot) {
  document.getElementById('slots-section').style.display = 'none';
  const sec = document.getElementById('booking-section');
  sec.style.display = 'block';
  document.getElementById('slotId').value = slot.id;
  document.getElementById('slotInfo').value = `${slot.startTime} — ${slot.endTime}`;
  document.getElementById('name').value = '';
  document.getElementById('email').value = '';
  showMsg('');
}

document.getElementById('cancelBtn').addEventListener('click', () => {
  document.getElementById('booking-section').style.display = 'none';
  document.getElementById('slots-section').style.display = 'block';
});

document.getElementById('booking-form').addEventListener('submit', async (e) => {
  e.preventDefault();
  const slotId = document.getElementById('slotId').value;
  const name = document.getElementById('name').value.trim();
  const email = document.getElementById('email').value.trim();
  if (!name || !email) return showMsg('Name and email are required', true);

  const payload = { name, email };

  try {
    const res = await fetch(`${API}/slots/${slotId}/book`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });

    if (res.status === 409) {
      const err = await res.json();
      showMsg(err.message || 'Slot no longer available', true);
      await fetchSlots();
      document.getElementById('booking-section').style.display = 'none';
      document.getElementById('slots-section').style.display = 'block';
      return;
    }

    if (!res.ok) {
      const err = await res.json();
      showMsg(err.message || 'Booking failed', true);
      return;
    }

    const result = await res.json();
    showMsg(`Booked! Reservation id: ${result.bookingId}`, false);
    document.getElementById('booking-section').style.display = 'none';
    document.getElementById('slots-section').style.display = 'block';
    await fetchSlots();
  } catch (ex) {
    showMsg('Network error', true);
  }
});

function showMsg(text, isError) {
  const el = document.getElementById('message');
  el.style.color = isError ? 'crimson' : 'green';
  el.textContent = text;
}

fetchSlots();
