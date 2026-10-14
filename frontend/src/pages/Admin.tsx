import{useEffect,useState} from 'react';
import { api, message } from '../services/api';
export default function Admin() {

    const [c, setC] = useState<any>(),[groups, setGroups] = useState<any[]>([]),[teams, setTeams] = useState<any[]>([]),[stadiums, setStadiums] = useState<any[]>([]),[matches, setMatches] = useState<any[]>([]),[err, setErr] = useState('');

    async function load() {
        try {
            const [a, b, d, e, f] = await Promise.all([api.get('/admin/championship'), api.get('/admin/groups'), api.get('/admin/teams'), api.get('/admin/stadiums'), api.get('/admin/matches')]);
            setC(a.data);
            setGroups(b.data);
            setTeams(d.data);
            setStadiums(e.data);
            setMatches(f.data);
        }
        catch (x) { setErr(message(x)) }
    }

    useEffect(() => { load() }, []);

    async function saveChamp(e: any) {
        e.preventDefault();
        const f = new FormData(e.currentTarget);
        await api.put('/admin/championship/' + c.id, { name: f.get('name'), location: f.get('location'), startsAt: f.get('startsAt'), endsAt: f.get('endsAt'), discount10Until: f.get('discount10Until'), description: f.get('description') });
        load()
    }

    async function addGroup(e: any) {
        e.preventDefault();
        const f = new FormData(e.currentTarget);
        await api.post('/admin/groups', { code: f.get('code'), championshipId: c.id });
        e.currentTarget.reset();
        load()
    }

    async function addTeam(e: any) {
        e.preventDefault();
        const f = new FormData(e.currentTarget);
        await api.post('/admin/teams', { name: f.get('name'), playerCount: +f.get('playerCount')!, coach: f.get('coach'), groupId: +f.get('groupId')!, wins: 0, losses: 0, points: 0 });
        e.currentTarget.reset();
        load()
    }

    async function addStadium(e: any) {
        e.preventDefault();
        const f = new FormData(e.currentTarget);
        await api.post('/admin/stadiums', { name: f.get('name'), city: f.get('city'), address: f.get('address'), capacity: +f.get('capacity')! }); e.currentTarget.reset();
        load()
    }

    async function addMatch(e: any) {
        e.preventDefault();
        const f = new FormData(e.currentTarget);
        await api.post('/admin/matches', { startsAt: f.get('startsAt'), basePriceEur: +f.get('price')!, stadiumId: +f.get('stadiumId')!, team1Id: +f.get('team1Id')!, team2Id: +f.get('team2Id')! });
        e.currentTarget.reset();
        load()
    }

    if (!c) return <p>Učitavanje...</p>;
    return (<section>
        <h2>Administracija</h2>
        {err && <p className="error">{err}</p>}
        <article className="card">
            <h3>Osnovne informacije</h3>
            <form onSubmit={saveChamp} className="formgrid">
                <input name="name" defaultValue={c.name} />
                <input name="location" defaultValue={c.location} />
                <input name="startsAt" type="date" defaultValue={c.startsAt?.slice(0, 10)} />
                <input name="endsAt" type="date" defaultValue={c.endsAt?.slice(0, 10)} />
                <input name="discount10Until" type="date" defaultValue={c.discount10Until?.slice(0, 10)} />
                <input name="description" defaultValue={c.description} />
                <button>Sačuvaj</button>
            </form>
        </article>
        <div className="grid">
            <article className="card">
                <h3>Grupe</h3>
                <form onSubmit={addGroup} className="inline">
                    <input name="code" placeholder="Oznaka" />
                    <button>Dodaj</button>
                </form>{groups.map(g => <p key={g.id}>{g.code}</p>)}
            </article>
            <article className="card">
                <h3>Timovi</h3>
                <form onSubmit={addTeam}>
                    <input name="name" placeholder="Naziv" />
                    <input name="coach" placeholder="Trener" />
                    <input name="playerCount" type="number" placeholder="Broj igrača" />
                    <select name="groupId">{groups.map(g => <option value={g.id}>{g.code}</option>)}</select>
                    <button>Dodaj</button>
                </form>{teams.map(t => <p key={t.id}>{t.name}</p>)}
            </article>
            <article className="card">
                <h3>Stadioni</h3>
                <form onSubmit={addStadium}>
                    <input name="name" placeholder="Naziv" />
                    <input name="city" placeholder="Grad" />
                    <input name="address" placeholder="Adresa" />
                    <input name="capacity" type="number" placeholder="Kapacitet" />
                    <button>Dodaj</button>
                </form>{stadiums.map(s => <p key={s.id}>{s.name} ({s.capacity})</p>)}
            </article>
            <article className="card">
                <h3>Utakmice</h3>
                <form onSubmit={addMatch}>
                    <input name="startsAt" type="datetime-local" />
                    <input name="price" type="number" step="0.01" placeholder="Cena EUR" />
                    <select name="stadiumId">{stadiums.map(s => <option value={s.id}>{s.name}</option>)}</select>
                    <select name="team1Id">{teams.map(t => <option value={t.id}>{t.name}</option>)}</select>
                    <select name="team2Id">{teams.map(t => <option value={t.id}>{t.name}</option>)}</select>
                    <button>Dodaj</button>
                </form><p>Ukupno: {matches.length}</p>
            </article>
        </div>
    </section>);
}