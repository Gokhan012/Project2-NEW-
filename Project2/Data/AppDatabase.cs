using SQLite;
using Project2.Models;

namespace Project2.Data;

public class AppDatabase
{
    private readonly SQLiteAsyncConnection _db;
    private bool _initialized;

    public AppDatabase(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
    }

    private async Task InitAsync()
    {
        if (_initialized) return;
        await _db.CreateTableAsync<Person>();
        await _db.CreateTableAsync<tblWater>();
        await _db.CreateTableAsync<tblMedicine>();
        _initialized = true;
    }

    public async Task<List<Person>> GetPersonsAsync()
    {
        await InitAsync();
        return await _db.Table<Person>().ToListAsync();
    }


    public async Task<tblWater> GetFirstWaterRecordAsync()
    {
        await InitAsync();
        return await _db.Table<tblWater>().FirstOrDefaultAsync();
    }

    public async Task<Person?> GetByEmailAsync(string email)
    {
        await InitAsync();
        return await _db.Table<Person>().Where(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task<int> SaveMedicineAsync(tblMedicine medicine)
    {
        await InitAsync();

        // Eğer ID'si varsa (0 değilse) bu eski bir kayıttır, GÜNCELLE
        if (medicine.MedicineID != 0)
        {
            return await _db.UpdateAsync(medicine);
        }
        // Eğer ID'si 0 ise bu yeni bir kayıttır, EKLE
        else
        {
            return await _db.InsertAsync(medicine);
        }
    }

    // Giriş yapan kullanıcının PersonID'sine göre ilaç listesini getirir
    public async Task<List<tblMedicine>> GetMedicinesAsync(int personId)
    {
        await InitAsync(); // Veritabanı bağlantısının hazır olduğundan emin ol

        // PersonID'si eşleşenleri filtrele ve listele
        return await _db.Table<tblMedicine>()
                        .Where(i => i.PersonID == personId)
                        .ToListAsync();
    }

    // Seçilen ilacı veritabanından tamamen siler
    public async Task<int> DeleteMedicineAsync(tblMedicine medicine)
    {
        await InitAsync();
        return await _db.DeleteAsync(medicine);
    }
    public async Task<int> AddPersonAsync(Person p)
    {
        await InitAsync();
        return await _db.InsertAsync(p);
    }

    public async Task<int> UpdatePersonAsync(Person p)
    {
        await InitAsync();
        return await _db.UpdateAsync(p);
    }

    public async Task<int> DeletePersonAsync(Person p)
    {
        await InitAsync();
        return await _db.DeleteAsync(p);
    }
    public Task<int> SaveWaterAsync(tblWater water)
    {
        return _db.InsertAsync(water);
    }

    public Task<int> UpdateWaterAsync(tblWater water)
    {
        return _db.UpdateAsync(water);
    }
}
