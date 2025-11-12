## Onion Architecture
เป็นสภาปัตยกรรม ที่แบ่งการทำงานในส่วนต่าง ๆ ออกมาเป็นชั้น ๆ โดยมีหัวใจสำคัญ คือ <mark>ต้องการให้ Core Logic ของแอพพลิเคชัน ถูกแยกออกจากการพึ่งพาเทคโนโลยีภายนอก เช่น การติดต่อกับฐานข้อมูล</mark>

<p align="center">
  <img src="./images/01/1.png" alt="onion architecture layer" width="400"/>
</p>

จากรูปภาพดังกล่าว วัตถุประสงค์หลักของสถาปัตยกรรมนี้ คือ <mark>การปกป้อง domain layer และ service layer ให้ไม่ได้รับผลกระทบจากการเปลี่ยนแปรงเทคโนโลยีในชั้น Infrastructure, Presenstation</mark>

กฏเหล็กของ onion architecture คือ <mark>layer ด้านนอกจะต้องพี่งพา layer ด้านในเสมอ และ layer ด้านใน จะต้องไม่พึ่งพา layer ด้านนอก</mark>

### รายละเอียดแต่ละชั้น
1. **Domain** จัดเก็บกฏทางธุรกิจ
2. **Service** จัดเก็บกระบวนการของระบบ
3. **Infrastructure** จัดเก็บเครื่องมือเพิ่มเติม เช่น การติดต่อกับฐานข้อมูล การส่งอีเมล
4. **Presentation** จัดเก็บส่วนที่ผู้ใช้สามารถเข้าถึงได้

*Infrastructure และ Presentation อยู่ในชั้นเดียวกัน*

## รายละเอียดแต่ละชั้น (แบบละเอียด)
### Domain Layer
เป็นหัวใจหลักของแอปพลิเคชัน โดยประกอบด้วย
- Core Business Logic
- Business Rule
- Entity
- Value Object

โดยใน layer จะเป็นอิสระอย่างสมบูรณ์ ไม่พึ่งพา layer อื่น ๆ (เพื่อให้แน่ใจว่า การเปลี่ยนแปรงของเทคโนโลยี จะไม่ส่งผลต่อตรรกะทางธุรกิจ)

**ตัวอย่าง**
    
    public class BankAccount
    {
        public decimal Balance { get; private set; }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("จำนวนเงินต้องมากกว่า 0");

            if (this.Balance - amount < 0)
                thorw new InvalidOperationException("ยอดเงินไม่เพียงพอสำหรับการถอน")

            this.Balance - amount;
        }
    }

Core Business Logic ของชุดคำสั่งดังกล่าว คือ
1. ห้ามถอนเงินเกินยอดเงินคงเหลือ
2. ห้ามถอนเงินจำนวนติดลบ

## Service Layer
ทำหน้าที่ <mark>ควบคุมกระบวนการทางธุรกิจ</mark> เช่น *จากตัวอย่างด้านบน เมื่อใน BankAccount ที่อยู่ใน Domain layer มีการส่งคือข้อผิดพลาด "จำนวนเงินต้องมากกว่า 0" ชั้น Service จะมีหน้าที่จัดการว่า ถ้าหากมีข้อผิดพลาดนี้เกิดขึ้น จะต้องทำอย่างไร*

และคอยประสานงานการทำงานระหว่าง Domain layer และ Infrastructure Layer ผ่าน interface

โดยในชั้นนี้จะ <mark>ไม่มี logic ที่เกี่ยวข้องกับเครื่องมืออื่น ๆ เช่น SQL, Http Request, Email send</mark>

**ตัวอย่าง**

    public class BankAccountService
    {
        private readonly ILogger _logger;
        private readonly IBankAccountRepository _bankAccountRepo;

        public BankAccountService(IBankAccountRepository bankAccountRepo, ILogger logger)
        {
            this._bankAccountRepo = bankAccountRepo;
            this._logger = logger;
        }

        public void Withdraw(decimal amount)
        {
            try 
            {
                BankAccount bankAccount = new BankAccount();

                this.bankAccount.Withdraw(amount);

                this._bankAccountRepo.Update(bankAccount);
            }
            catch (Exception ex)
            {
                this._logger.Create(ex);
                throw new Exception(ex.Message);
            }
        }
    }

โดยเมื่อชั้น Service ต้องการติดต่อกับฐานข้อมูลหรืออื่น ๆ มันจะกำหนด interface ขึ้นมาในชั้นนี้เอง และให้ infrastructure รับผิดชอบวิธีการที่จะติดต่อกับโลกภายนอกตามที่ interface ต้องการ

<mark>เช่น จากชุดคำสั่งด้านบน service ไม่จำเป็นต้องรู้ว่าขึ้นตอนการลง log คืออะไรบ้าง รู้แค่ว่าสามารถลง log ได้ด้วยการเรียกใช้ method นี้ก็เพียงพอ ส่วนเรื่องการทำงานในการลง log (method Create()) จะปล่อยให้ infrastructure เป็นผู้จัดการ</mark>

*พึ่งพาเฉพาะ Domain Layer เท่านั้น*

## Infrastructure Layer
เป็นชั้นที่ทำหน้าที่ <mark>จัดการเรื่องเทคโนโลยีและการเชื่อมต่อกับโลกภายนอก</mark> เช่น
- Database
- File System
- External API
- Logger

**ตัวอย่าง**

จากชุดคำสั่งก่อนหน้าใน Service Layer พบว่า มีการร้องขอเรียกใช้การจัดการฐานข้อมูล (IBankAccountRepository) และการลง log (ILogger) เราจึงจำเป็นต้องนำการร้องขอนั้นมี implements ในชั้นนี้

BankAccountRespository.cs

    // interface IBackAccountRepository มาจากชั้น Service Layer
    public class BackAccountRepository : IBackAccountRepository 
    {
        private readonly AppDbContext _context;

        public BackAccountRepository(AppDbContext _context)
        {
            this._context = context;
        }

        public void Update(BankAccount bankAccount)
        {
            this._context.BankAccount.Update(bankAccount);
            this._context.SaveChanges();
        }
    }

*พึ่งพาเฉพาะ Application/Service layer และ Infrastructure (บางกรณีอาจต้องตั้งค่า dependencies)*

## Presentation Layer
เป็นชั้นที่ทำหน้าที่ <mark>ผู้ใช้ติดต่อโดยตรงกับระบบ</mark> โดยมากจะอยู่ในรูปแบบ
- Web UI
- Rest API
- Console App

โดยทำหน้าที่ รับ input จากผู้ใช้ => ส่งต่อให้ Service => รับผลตอบกลับจาก service และส่งให้ผู้ใช้

**ตัวอย่าง**

    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(IBankAccountController bankAccountService)
        {
            this._bankAccountService = bankAccountService
        }

        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBodt] RqWithdrawDto rq)
        {
            try 
            {
                this._bankAccountService.Withdraw(rq.Amount);

                return Ok(new BaseResponse<object>()
                {
                    IsSuccess = true,
                    Message = "Successful",
                    Data = null
                })
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<object>()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }

*พึ่งพาเฉพาะ Domain layer (เพื่อเข้าถึง entity) และ Service layer (เพื่อเข้าถึง interface ที่ service ต้องการให้มีการ implement)*