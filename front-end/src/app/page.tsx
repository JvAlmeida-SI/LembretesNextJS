'use client';

import React, { useState, useEffect } from "react";
import moment from "moment";
import axios from "axios";

  
  const ReminderApp = () => {
    const [reminders, setReminders] = useState<Reminder[]>([]);
    const [newReminderName, setNewReminderName] = useState<string>("");
    const [newReminderDate, setNewReminderDate] = useState<string>("");
    const [error, setError] = useState<string | null>(null);
    
    interface Reminder {
    tarefas: {id:number, nomeTarefa:string, dataTarefa:string}[];
    data: string;
  }

    const carregarLembretes = async (): Promise<void> => {
      try {
        const response = await axios.get('http://localhost:5146/tarefa');
        setReminders(response.data);
      } catch (error) {
        console.error(error);
      }
    };


    const excluirTarefa = (id: number) => {
      axios.delete(`http://localhost:5146/tarefa/${id}`)
        .then(response => {
          console.log('Tarefa excluída com sucesso.');
          carregarLembretes();
        })
        .catch(error => {
          console.error('Erro ao excluir a tarefa:', error);
        });
    };  
    

    useEffect(() => {carregarLembretes();}, []);

    const addReminder = async () => {
      if (!newReminderName.trim()) {
        setError("O nome do lembrete é obrigatório.");
        return;
      }


      try {
        const newReminder = {
          nome_tarefa: newReminderName,
          data_tarefa: newReminderDate,
        };
        

        const response = await axios.post('http://localhost:5146/tarefa', newReminder);

        await carregarLembretes();
        setNewReminderName("");
        setNewReminderDate("");
        setError(null);
      } catch (error) {
        console.error(error);
      }
    };

    return (
      <div className="reminder-app">
      <h1 className="title">Novo lembrete</h1>
      <div className="form">
        <label>
          Nome:
          <input
            type="text"
            value={newReminderName}
            onChange={(e) => setNewReminderName(e.target.value)}
          />
        </label>
        <label>
          Data:
          <input
            type="date"
            value={newReminderDate}
            onChange={(e) => setNewReminderDate(e.target.value)}
          />
        </label>
        {error && <div className="error">{error}</div>}
        <button className="add-button" onClick={addReminder}>
          Adicionar
        </button>
      </div>
      <h2 className="title">Lista de lembretes</h2>
      {reminders.map((reminder) => (
        <div key={reminder.data}>
          <h3>{moment(reminder.data).format('DD/MM/YYYY')}</h3>
          <ul className="reminder-list">
            {reminder.tarefas.map((tarefa) => (
              <li key={tarefa.id} className="reminder-item">
                {tarefa.nomeTarefa}
                <button
                  className="delete-button"
                  onClick={() => excluirTarefa(tarefa.id)}>X</button>
              </li>
            ))}
          </ul>
        </div>
      ))}
    </div>
    );
  };
  export default ReminderApp;