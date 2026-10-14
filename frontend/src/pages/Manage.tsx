import { useEffect, useState } from 'react';
import { api, message } from '../services/api';
export default function Manage() {
    const [email, setEmail] = useState(''), [code, setCode] = useState(''), [ticket, setTicket] = useState<any>(), [matches, setMatches] = useState<any[]>([]), [matchId, setMatchId] = useState(0), [seats, setSeats] = useState(1), [err, setErr] = useState('');
    useEffect(() => {
        api.get('/public/matches').then(r => {
            setMatches(r.data);
            if (r.data[0]) setMatchId(r.data[0].id)
        })
    }, []);

    async function find() {
        try {
            setTicket((await api.post('/tickets/find', { email, accessCode: code })).data);
            setErr('')
        }
        catch (e) { setErr(message(e)) }
    }

    async function cancel() {
        if (!window.confirm('Da li sigurno otkazujete kartu?')) return;
        try {
            await api.post('/tickets/cancel', { email, accessCode: code });
            await find()
        }
        catch (e) { setErr(message(e)) }
    }

    async function remove(itemId: number) {
        try {
            setTicket((await api.put('/tickets', { email, accessCode: code, addItems: [], removeItemIds: [itemId] })).data)
        }
        catch (e) { setErr(message(e)) }
    }

    async function add() {
        try {
            setTicket((await api.put('/tickets', { email, accessCode: code, addItems: [{ matchId, seatCount: seats }], removeItemIds: [] })).data);
            setErr('')
        }
        catch (e) { setErr(message(e)) }
    }

    return (<section>
        <h2>Izmena i otkazivanje karte</h2>
        <div className="inline">
            <input value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" />
            <input value={code} onChange={e => setCode(e.target.value)} placeholder="Šifra karte" />
            <button onClick={find}>Pronađi</button>
        </div>{err && <p className="error">{err}</p>}{ticket &&
            <article className="card">
                <h3>Karta {ticket.accessCode}</h3>
                <p>Status: {ticket.status} · Ukupno: {ticket.totalAmount} {ticket.currency}</p>
                {ticket.items.map((i: any) =>
                    <div className="match" key={i.id}>
                        <span>{i.match} — {i.seatCount} mesta</span>
                        {ticket.status === 'Active' &&
                            <button className="danger" onClick={() => remove(i.id)}>Ukloni</button>
                        }
                    </div>
                )}
                {ticket.status === 'Active' && <><h4>Dodaj utakmicu</h4>
                    <div className="inline">
                        <select value={matchId} onChange={e => setMatchId(+e.target.value)}>
                            {matches.map(m =>
                                <option key={m.id} value={m.id}>{m.team1} - {m.team2}</option>
                            )}
                        </select>
                        <input type="number" min="1" max="20" value={seats} onChange={e => setSeats(+e.target.value)} />
                        <button onClick={add}>Dodaj</button>
                    </div>
                    <button className="danger" onClick={cancel}>Otkaži kartu</button>
                </>
                }
            </article>}
    </section>);
}
