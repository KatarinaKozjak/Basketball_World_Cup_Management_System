import { useEffect, useState } from 'react';
import { api, message } from '../services/api';
export default function Buy() {
    const [matches, setMatches] = useState<any[]>([]), [curr, setCurr] = useState<any[]>([]), [selected, setSelected] = useState<Record<number, number>>({}), [result, setResult] = useState<any>(), [err, setErr] = useState('');
    useEffect(() => {
        Promise.all([api.get('/public/matches'), api.get('/public/currencies')]).then(([m, c]) => {
            setMatches(m.data); setCurr(c.data)
        })
    }, []);

    async function submit(e: any) {
        e.preventDefault();
        setErr('');
        const f = new FormData(e.currentTarget);
        try {
            const r = await api.post('/tickets', { firstName: f.get('firstName'), lastName: f.get('lastName'), address: f.get('address'), postalCode: f.get('postalCode'), city: f.get('city'), country: f.get('country'), email: f.get('email'), emailConfirmation: f.get('emailConfirmation'), currencyCode: f.get('currencyCode'), promoCode: f.get('promoCode') || null, items: Object.entries(selected).filter(([, n]) => n > 0).map(([id, n]) => ({ matchId: +id, seatCount: n })) });
            setResult(r.data)
        }
        catch (x) { setErr(message(x)) }
    }

    return (<section>
        <h2>Kupovina karata</h2>
        {result ?
            <div className="success">
                <h3>Karta je kreirana</h3>
                <p>Šifra: <b>{result.accessCode}</b></p>
                <p>Promo-kod: <b>{result.generatedPromoCode}</b></p>
                <p>Ukupno: {result.totalAmount} {result.currency}</p>
            </div>
            :
            <form onSubmit={submit}>
                <div className="formgrid">
                    <input name="firstName" placeholder="Ime" required />
                    <input name="lastName" placeholder="Prezime" required />
                    <input name="address" placeholder="Adresa" required />
                    <input name="postalCode" placeholder="Poštanski broj" required />
                    <input name="city" placeholder="Mesto" required />
                    <input name="country" placeholder="Država" required />
                    <input name="email" type="email" placeholder="Email" required />
                    <input name="emailConfirmation" type="email" placeholder="Potvrda emaila" required />
                    <select name="currencyCode">{curr.map(c => <option key={c.id}>{c.code}</option>)}</select>
                    <input name="promoCode" placeholder="Promo-kod (opciono)" />
                </div>
                <h3>Odaberi utakmice</h3>
                {matches.map(m => <label className="match" key={m.id}>
                    <span>{m.team1} - {m.team2}<small>{
                        new Date(m.startsAt).toLocaleString()} · {m.stadium} · {m.basePriceEur} EUR · slobodno {m.capacity - m.sold}</small>
                    </span>
                    <input type="number" min="0" max="20" value={selected[m.id] ?? 0} onChange={e => setSelected({ ...selected, [m.id]: +e.target.value })} />
                </label>)
                }
                {
                    err && <p className="error">{err}</p>
                }
                <button>Kupi kartu</button>
            </form>
        }
    </section>);
}