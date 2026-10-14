import { useEffect, useState } from 'react';
import { api } from '../services/api';
export default function Home() {
    const [d, setD] = useState<any>();
    useEffect(() => { api.get('/public/home').then(r => setD(r.data)) }, []);
    if (!d) return <p>Učitavanje...</p>;
    return (<section>
        <div className="hero">
            <h2>{d.championship?.name}</h2>
            <p>{d.championship?.location}</p>
            <p>{new Date(d.championship?.startsAt).toLocaleDateString()} – {new Date(d.championship?.endsAt).toLocaleDateString()}</p>
            <p>{d.championship?.description}</p>
        </div>
        <h2>Timovi po grupama</h2>
        <div className="grid">{d.groups?.map((g: any) =>
            <article className="card" key={g.id}>
                <h3>{g.code}</h3>
                {g.teams.map((t: any) => <p key={t.id}>
                    {t.name} — {t.points} poena</p>)
                }
            </article>)
        }
        </div>
    </section>);
}