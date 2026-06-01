import { useEffect, useState } from 'react';

export default function App() {
  const [firstNumber, setFirstNumber] = useState('12');
  const [secondNumber, setSecondNumber] = useState('30');
  const [result, setResult] = useState(null);
  const [error, setError] = useState('');
  const [welcomeMessage, setWelcomeMessage] = useState('Loading message...');

  useEffect(() => {
    async function loadWelcomeMessage() {
      try {
        const response = await fetch('/api/welcome');
        if (!response.ok) throw new Error();
        const data = await response.json();
        setWelcomeMessage(data.message);
      } catch {
        setWelcomeMessage('Hello World (Local)');
      }
    }
    loadWelcomeMessage();
  }, []);

  useEffect(() => {
    const controller = new AbortController();

    if (firstNumber.trim() === '' || secondNumber.trim() === '') {
      setResult(null);
      setError('');
      return () => controller.abort();
    }

    async function loadSum() {
      try {
        const response = await fetch(`/api/add?a=${firstNumber}&b=${secondNumber}`, {
          signal: controller.signal
        });

        if (!response.ok) {
          throw new Error('Unable to load the sum from the C# service.');
        }

        const data = await response.json();
        setResult(data.result);
      } catch (fetchError) {
        if (fetchError.name !== 'AbortError') {
          setError(fetchError.message);
        }
      }
    }

    loadSum();

    return () => controller.abort();
  }, [firstNumber, secondNumber]);

  return (
    <main className="shell">
      <section className="card">
        {/* <h1 style={{ paddingBottom: '20px' }}>Hello World</h1> */}
        <h1 style={{ paddingBottom: '20px' }}>{welcomeMessage}</h1>

        <div>
          <div> 
            <label className="field">
                <span className="label">First number </span>
                <input
                className="input"
                type="number"
                value={firstNumber}
                onChange={(event) => setFirstNumber(event.target.value)}
                />
            </label>
          </div>
        <div >
          <label className="field">
            <span className="label">Second number </span>
            <input
              className="input"
              type="number"
              value={secondNumber}
              onChange={(event) => setSecondNumber(event.target.value)}
            />
          </label>
        </div>

          <p className="label">Result</p>
          {error ? <p className="error">{error}</p> : <p className="sum">{result ?? 'Loading...'}</p>}
        </div>
      </section>
    </main>
  );
}
