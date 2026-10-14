import { useEffect, useState } from 'react';
import { api } from '../services/api';
export default function Reports() {
    const [m, setM] = useState<any[]>([]), [d, setD] = useState<any[]>([]);
    useEffect(() => {
        api.get('/reports/by-match').then(r => setM(r.data));
        api.get('/reports/by-day').then(r => setD(r.data))
    }, []);

    const maxM = Math.max(1, ...m.map(x => x.ticketsSold));
    const maxD = Math.max(1, ...d.map(x => x.tickets));
    return (<section>
        <h2>Portal za izveštavanje</h2>
        <article className="card">
            <h3>Broj kupljenih mesta po utakmicama</h3>
            {m.map(x =>
                <div className="barrow" key={x.id}>
                    <span>{x.match}</span>
                    <div className="bar" style={{ width: `${x.ticketsSold / maxM * 100}%` }}>{x.ticketsSold}</div>
                </div>
            )}
        </article>
        <article className="card">
            <h3>Broj kupovina po danima</h3>
            {d.map((x, i) =>
                <div className="barrow" key={i}>
                    <span>{new Date(x.date).toLocaleDateString()}</span>
                    <div className="bar" style={{ width: `${x.tickets / maxD * 100}%` }}>
                        {x.tickets}
                    </div>
                </div>
            )}
        </article>
    </section>);
}
